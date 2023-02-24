using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;

namespace BossRush
{
    public class GUIBox
    {
        public Vector2 topLeftCorner;
        public OptionCategory contents;
        public int fontSize;
        public float gap;

        public GUIBox(Vector2 topLeftCorner, OptionCategory contents, int fontSize = 20, float boxLeaway = 10f)
        {
            this.topLeftCorner = topLeftCorner;
            this.contents = contents;
            this.fontSize = fontSize;
            gap = boxLeaway;
        }

        public void OnGUI()
        {
            var prevClip = GUI.skin.label.clipping;
            GUI.skin.label.clipping = TextClipping.Overflow;

            var prevColor = GUI.color;
            GUI.color = Color.gray;

            var size = contents.CalcSize(fontSize);

            GUI.Box(new Rect(new Vector2(topLeftCorner.x - gap, topLeftCorner.y - gap), size + topLeftCorner + new Vector2(gap, gap) * 2), "");

            GUI.color = prevColor;

            contents.OnGUI(topLeftCorner, 1, 0, fontSize);
            GUI.skin.label.clipping = prevClip;
        }
    }

    public abstract class BaseOption
    {
        public float width;
        public abstract Vector2 Update(Vector2 startCorner, float scale, float height);

        public abstract float CalcHeight();
    }

    public class ButtonOption : BaseOption
    {
        public float height;
        public string text;
        public bool pressed = false;
        public float scale = 1;

        public ButtonOption(string text, float width)
        {
            base.width = width;
            height = 0;
            this.text = text;
        }

        public bool IsPressed()
        {
            if (pressed)
            {
                pressed = false;
                return true;
            }
            return false;
        }

        public override Vector2 Update(Vector2 startCorner, float scale, float height)
        {
            this.height = height * 2;
            this.scale = scale;

            var rect = new Rect(startCorner.x, startCorner.y, base.width * scale, this.height * scale);
            if (GUI.Button(rect, text))
            {
                pressed = true;
            }
            return startCorner + new Vector2(0, this.height * scale);
        }

        public override float CalcHeight()
        {
            return height * scale;
        }
    }

    public class ToggleOption : BaseOption
    {
        public bool state;
        public float height;
        public string text;
        public float scale = 1;

        public ToggleOption(string text, float width, bool state = false) 
        {
            this.text = text;
            base.width = width;
            height = 0;
            this.state = state;
        }

        public bool getState()
        {
            return this.state;
        }

        public override Vector2 Update(Vector2 startCorner, float scale, float height)
        {
            this.height = height * 1.2f;
            this.scale = scale;

            var rect = new Rect(startCorner.x, startCorner.y, base.width * scale, this.height * scale);
            state = GUI.Toggle(rect, state, text);

            return startCorner + new Vector2(0, this.height * scale);
        }

        public override float CalcHeight()
        {
            return height * scale;
        }
    }

    public class NumberBoxOption : BaseOption
    {
        public int state;
        public float height;
        public string text;
        public float scale = 1;

        public NumberBoxOption(string text, float width, int state = 0)
        {
            this.text = text;
            base.width = width;
            height = 0;
            this.state = state;
        }

        public int getState()
        {
            return state;
        }

        public override Vector2 Update(Vector2 startCorner, float scale, float height)
        {
            this.height = height * 1.5f;
            this.scale = scale;

            var style = new GUIStyle();
            style.font = GUI.skin.font;
            style.fontSize = GUI.skin.label.fontSize;
            var textRect = new Rect(startCorner.x, startCorner.y, style.CalcSize(new GUIContent(text + " ")).x, this.height * scale);
            GUI.Label(textRect, text);

            var fieldRect = new Rect(textRect.x + textRect.width, textRect.y, base.width * scale, this.height * scale);

            string newState;
            if (state != 0)
            {
                newState = Regex.Replace(GUI.TextField(fieldRect, state.ToString()), @"[^0-9 ]", "");
            }
            else
            {
                newState = Regex.Replace(GUI.TextField(fieldRect, ""), @"[^0-9 ]", "");
            }
            if (newState != "")
            {
                state = int.Parse(newState);
            }
            else
            {
                state = 0;
            }
            return startCorner + new Vector2(0, this.height * scale);
        }

        public override float CalcHeight()
        {
            return height * scale;
        }
    }

    public class SelectionGridOption : BaseOption
    {
        public int state;
        public float height;
        public string[] texts;
        public float scale;

        public SelectionGridOption(string[] texts, float width, int state = 0)
        {
            this.texts = texts;
            base.width = width;
            height = 0;
            this.state = state;
        }

        public int getState()
        {
            return state;
        }

        public override Vector2 Update(Vector2 startCorner, float scale, float height)
        {
            this.height = height * 2;
            this.scale = scale;

            var rect = new Rect(startCorner.x, startCorner.y, base.width * scale, this.height * texts.Count() * scale);
            state = GUI.SelectionGrid(rect, state, texts, 1);
            return startCorner + new Vector2(0, rect.height);
        }

        public override float CalcHeight()
        {
            return height * scale * texts.Count();
        }
    }

    public class OptionCategory
    {
        public OptionCategoryType type;
        public OptionCategory[] subCategories;
        public BaseOption[] options;
        public string title;
        public float gap;
        public float scale;

        /// <summary>
        /// Has to have one and only one of the list parameters given.
        /// Leaving gap or scale to the default values will copy them from the parent category.
        /// </summary>
        public OptionCategory(string title = "", BaseOption[] options = null, OptionCategory[] subCategories = null, float gapBetweenThings = 0, float scale = 1)
        {
            if (subCategories == null && options == null || (subCategories != null && options != null)) { throw new Exception("OptionCategory has to have one and only one of the list parameters given. Title of category: " + title); }

            if (subCategories == null)
            {
                type = OptionCategoryType.optionHolder;          
            }
            else
            {
                type = OptionCategoryType.subHolder;
            }
            this.options = options;
            this.subCategories = subCategories;
            this.title = title;
            gap = gapBetweenThings;
            this.scale = scale;
        }

        public Vector2 OnGUI(Vector2 startCorner, float scale, float gap, int fontSize)
        {
            ChangeFontSize((int)(fontSize * scale));

            if (this.scale == 1) { this.scale = scale; }
            if (this.gap == 0) { this.gap = gap; }

            if (this.title != "")
            {
                GUI.skin.label.fontSize *= 2;

                var style = new GUIStyle();
                style.font = GUI.skin.font;
                style.fontSize = GUI.skin.label.fontSize;
                var size = style.CalcSize(new GUIContent(title));
                var textRect = new Rect(startCorner.x, startCorner.y, size.x * scale, size.y * scale);
                GUI.Label(textRect, title);
                startCorner += new Vector2(0, gap*3 + size.y);
                GUI.skin.label.fontSize /= 2;
            }


            if (type == OptionCategoryType.subHolder)
            {
                return updateSubs(startCorner, fontSize);
            }

            ChangeFontSize(12);
            return updateOptions(startCorner, fontSize);
        }

        public Vector2 updateSubs(Vector2 startCorner, int fontSize)
        {
            Vector2 updatingCorner = startCorner;
            foreach (OptionCategory category in subCategories)
            {
                updatingCorner = category.OnGUI(updatingCorner, scale, gap, fontSize);
            }
            return updatingCorner;
        }

        public Vector2 updateOptions(Vector2 startCorner, float height)
        {
            Vector2 updatingCorner = startCorner;
            foreach (BaseOption option in options)
            {
                updatingCorner = option.Update(updatingCorner, scale, height) + new Vector2(0, gap);
            }
            return updatingCorner;
        }

        public Vector2 CalcSize(int fontSize) //return vector2 = width, height
        {
            ChangeFontSize((int)(fontSize * scale));

            Vector2 result = Vector2.zero;

            if (title != "")
            {
                GUI.skin.label.fontSize *= 2;

                var style = new GUIStyle();
                style.font = GUI.skin.font;
                style.fontSize = GUI.skin.label.fontSize;
                result = style.CalcSize(new GUIContent(title));

                GUI.skin.label.fontSize /= 2;
            }

            if (type == OptionCategoryType.optionHolder)
            {
                foreach (var o in options)
                {
                    if (o.width > result.x)
                    {
                        result.x = o.width;
                    }

                    result.y += o.CalcHeight() + gap;
                }
            }
            if (type == OptionCategoryType.subHolder)
            {
                foreach (var s in subCategories)
                {
                    var n = s.CalcSize(fontSize);
                    if (n.x > result.x)
                    {
                        result.x = n.x;
                    }
                    result.y += n.y;
                }
            }

            ChangeFontSize(12);
            return result;
        }

        public void ChangeFontSize(int size)
        {
            GUI.skin.button.fontSize = size;
            GUI.skin.label.fontSize = size;
            GUI.skin.scrollView.fontSize = size;
            GUI.skin.textArea.fontSize = size;
            GUI.skin.textField.fontSize = size;
            GUI.skin.toggle.fontSize = size;
        }
    }

    public enum OptionCategoryType
    {
        subHolder,
        optionHolder
    }
}