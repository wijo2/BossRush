using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;

namespace GUIBox
{
    public class GUIBox
    {
        public Vector2 topLeftCorner;
        public OptionCategory contents;
        public float gap;

        public GUIBox(Vector2 topLeftCorner, OptionCategory contents, float boxLeaway = 10f)
        {
            this.topLeftCorner = topLeftCorner;
            this.contents = contents;
            gap = boxLeaway;
        }

        public void OnGUI()
        {
            var prevClip = GUI.skin.label.clipping;
            GUI.skin.label.clipping = TextClipping.Overflow;

            var prevColor = GUI.color;
            GUI.color = Color.gray;

            var size = contents.CalcSize(20, 5);

            GUI.Box(new Rect(new Vector2(topLeftCorner.x - gap, topLeftCorner.y - gap), size + new Vector2(gap * 2, gap)), "");

            GUI.color = prevColor;

            contents.OnGUI(topLeftCorner, 5, 20);
            GUI.skin.label.clipping = prevClip;
        }

        public static Vector2 CalcTextSize(string text, int fontSize)
        {
            var style = new GUIStyle();
            style.font = GUI.skin.font;
            style.fontSize = fontSize;
            return style.CalcSize(new GUIContent(text));
        }

        public static void ChangeFontSize(int size)
        {
            GUI.skin.button.fontSize = size;
            GUI.skin.label.fontSize = size;
            GUI.skin.scrollView.fontSize = size;
            GUI.skin.textArea.fontSize = size;
            GUI.skin.textField.fontSize = size;
            GUI.skin.toggle.fontSize = size;
        }

        public static void EnableToggleResize()
        {
            GUI.skin.toggle.border = new RectOffset(0, 0, 0, 0);
            GUI.skin.toggle.overflow = new RectOffset(0, 0, 0, 0);
        }

        public static void ResetToggleResize()
        {
            GUI.skin.toggle.border = new RectOffset(14, 0, 14, 0);
            GUI.skin.toggle.overflow = new RectOffset(-1, 0, -4, 0);
        }
    }

    public abstract class BaseOption
    {
        public float width;
        public abstract Vector2 Update(Vector2 startCorner, int fontSize);

        public abstract float GetHeight();
        public abstract float GetWidth();
    }

    public class ButtonOption : BaseOption
    {
        public float height;
        public string text;
        public bool pressed = false;
        public float emptySpaceMultiplier;
        public float? overrideWidth = null;
        public float? overrideHeight = null;

        public ButtonOption(string text, float emptySpaceMultiplier = 1.5f, float? overrideWidth = null, float? overrideHeight = null)
        {
            width = 0;
            height = 0;
            this.text = text;
            this.emptySpaceMultiplier = emptySpaceMultiplier;
            this.overrideWidth = overrideWidth;
            this.overrideHeight = overrideHeight;
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

        public override Vector2 Update(Vector2 startCorner, int fontSize)
        {
            var r = GUIBox.CalcTextSize(text, fontSize);
            width = overrideWidth == null ? r.x * emptySpaceMultiplier : overrideWidth.Value;
            height = overrideHeight == null ? r.y * emptySpaceMultiplier : overrideHeight.Value;

            var rect = new Rect(startCorner.x, startCorner.y, width, height);
            if (GUI.Button(rect, text))
            {
                pressed = true;
            }
            return new Vector2(width, height);
        }

        public override float GetHeight()
        {
            return height;
        }

        public override float GetWidth()
        {
            return width;
        }
    }

    public class ToggleOption : BaseOption
    {
        public bool state;
        public float height;
        public string text;

        public ToggleOption(string text, bool initialState = false) 
        {
            this.text = text;
            width = 0;
            height = 0;
            state = initialState;
        }

        public bool GetState()
        {
            return state;
        }

        public override Vector2 Update(Vector2 startCorner, int fontSize)
        {
            var r = GUIBox.CalcTextSize(text, fontSize);
            height = r.y;
            width = r.x + r.y;

            var rect = new Rect(startCorner.x, startCorner.y, r.y * 0.8f, r.y * 0.8f);

            GUIBox.EnableToggleResize();
            state = GUI.Toggle(rect, state, "");
            GUIBox.ResetToggleResize();

            var lRect = new Rect(startCorner.x + r.y, startCorner.y - r.y * 0.1f, r.x, height);
            GUI.Label(lRect, text);

            return new Vector2(width, height);
        }

        public override float GetHeight()
        {
            return height;
        }

        public override float GetWidth()
        {
            return width;
        }
    }

    public class NumberBoxOption : BaseOption
    {
        public int state;
        public float height;
        public string text;
        public float? overrideHeight = null;

        public NumberBoxOption(float width, string text = "", float? overrideHeight = null, int initialState = 0)
        {
            this.text = text;
            this.width = width;
            height = 0;
            state = initialState;
            this.overrideHeight = overrideHeight;
        }

        public int GetState()
        {
            return state;
        }

        public override Vector2 Update(Vector2 startCorner, int fontSize)
        {
            height = overrideHeight == null ? fontSize * 1.5f : overrideHeight.Value;

            var textRect = new Rect(startCorner.x, startCorner.y, GUIBox.CalcTextSize(text, fontSize).x, height);
            GUI.Label(textRect, text);

            var fieldRect = new Rect(textRect.xMax, startCorner.y, width, height);

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
            return new Vector2(width, height);
        }

        public override float GetHeight()
        {
            return height;
        }

        public override float GetWidth()
        {
            return width;
        }
    }

    public class SelectionGridOption : BaseOption
    {
        public int state;
        public float height;
        public string[] texts;
        public float? overrideWidth = null;
        public float? overrideHeight = null;
        public float emptySpaceMultiplier;

        public SelectionGridOption(string[] texts, float emptySpaceMultiplier = 1.5f, int initialState = 0, float? overrideWidth = null, float? overrideHeight = null)
        {
            this.texts = texts;
            this.emptySpaceMultiplier = emptySpaceMultiplier;
            height = 0;
            state = initialState;

            this.overrideHeight = overrideHeight;
            this.overrideWidth = overrideWidth;
        }

        public int GetState()
        {
            return state;
        }

        public override Vector2 Update(Vector2 startCorner, int fontSize)
        {
            float biggestX = 0;
            float biggestY = 0;
            foreach (var text in texts)
            {
                var r = GUIBox.CalcTextSize(text, fontSize);
                if (r.x > biggestX)
                {
                    biggestX = r.x;
                }
                if (r.y > biggestY)
                {
                    biggestY = r.y;
                }
            }

            width = overrideWidth == null ? biggestX * emptySpaceMultiplier : overrideWidth.Value;
            height = overrideHeight == null ? biggestY * emptySpaceMultiplier : overrideHeight.Value;

            var rect = new Rect(startCorner.x, startCorner.y, width, height * texts.Count());
            state = GUI.SelectionGrid(rect, state, texts, 1);
            return new Vector2(width, rect.height);
        }

        public override float GetHeight()
        {
            return height * texts.Count();
        }
        public override float GetWidth()
        {
            return width;
        }
    }

    public class OptionCategory
    {
        public OptionCategoryType type;
        public OptionCategory[] subCategories;
        public BaseOption[] options;
        public string title;
        public float? gap;
        public int? fontSize;
        public float titleMulti;
        public bool active = true;

        /// <summary>
        /// Has to have one and only one of the list parameters given.
        /// Leaving gap or fontSize to the default value will copy it from the parent category.
        /// </summary>
        public OptionCategory(string title = "", BaseOption[] options = null, OptionCategory[] subCategories = null, float? gapBetweenThings = null, int? fontSize = null, float titleSizeMultiplier = 2)
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
            this.fontSize = fontSize;
            titleMulti = titleSizeMultiplier;
        }

        public Vector2 OnGUI(Vector2 startCorner, float gap, int fontSize)
        {
            if (!active) { return startCorner; }
            var tmpFontSize = this.fontSize == null ? fontSize : this.fontSize.Value;
            GUIBox.ChangeFontSize(tmpFontSize);
            var tmpGap = this.gap == null ? gap : this.gap.Value;

            if (this.title != "")
            {
                GUI.skin.label.fontSize = (int) (GUI.skin.label.fontSize * titleMulti);

                var style = new GUIStyle();
                style.font = GUI.skin.font;
                style.fontSize = GUI.skin.label.fontSize;
                var size = style.CalcSize(new GUIContent(title));

                var textRect = new Rect(startCorner.x, startCorner.y, size.x, size.y);
                GUI.Label(textRect, title);

                //GUI.Box(new Rect(startCorner, GUIBox.CalcTextSize(title, (int)(tmpFontSize * titleMulti))), ""); //debug

                startCorner += new Vector2(0, tmpGap*3 + size.y);

                GUI.skin.label.fontSize = tmpFontSize;
            }


            if (type == OptionCategoryType.subHolder)
            {
                return updateSubs(startCorner, tmpGap, tmpFontSize);
            }

            GUIBox.ChangeFontSize(12);
            return updateOptions(startCorner, tmpGap, tmpFontSize);
        }

        public virtual Vector2 updateSubs(Vector2 startCorner, float gap, int fontSize)
        {
            Vector2 updatingCorner = startCorner;
            foreach (OptionCategory category in subCategories)
            {
                //GUI.Box(new Rect(updatingCorner, category.CalcSize(fontSize, gap)), ""); //debug

                updatingCorner = category.OnGUI(updatingCorner, gap, fontSize) + new Vector2(0, gap);
            }
            return updatingCorner;
        }

        public virtual Vector2 updateOptions(Vector2 startCorner, float gap, int fontSize)
        {
            Vector2 updatingCorner = startCorner;
            Vector2 size;
            foreach (BaseOption option in options)
            {
                size = option.Update(updatingCorner, fontSize);
                updatingCorner += new Vector2(0, size.y + gap);
            }
            return updatingCorner;
        }

        public virtual Vector2 CalcSize(int fontSize, float gap) //return vector2 = width, height
        {
            if (!active) { return Vector2.zero; }
            var tmpFontSize = this.fontSize == null ? fontSize : this.fontSize.Value;
            GUIBox.ChangeFontSize(tmpFontSize);
            var tmpGap = this.gap == null ? gap : this.gap.Value;

            Vector2 result = Vector2.zero;

            if (title != "")
            {
                result = GUIBox.CalcTextSize(title, (int)(tmpFontSize * titleMulti)) + new Vector2(0, gap*3);
            }

            if (type == OptionCategoryType.optionHolder)
            {
                foreach (var o in options)
                {
                    var w = o.GetWidth();
                    if (w > result.x)
                    {
                        result.x = w;
                    }
                    result.y += o.GetHeight() + tmpGap;
                }
            }
            if (type == OptionCategoryType.subHolder)
            {
                foreach (var s in subCategories)
                {
                    var n = s.CalcSize(tmpFontSize, tmpGap);
                    if (n.x > result.x)
                    {
                        result.x = n.x;
                    }
                    result.y += n.y + tmpGap;
                }
            }

            GUIBox.ChangeFontSize(12);
            return result;
        }

        public void SetActive(bool active)
        {
            this.active = active;
        }
    }

    public class HorizontalOptionCategory : OptionCategory
    {
        /// <summary>
        /// Has to have one and only one of the list parameters given.
        /// Leaving gap or fontSize to the default value will copy it from the parent category.
        /// </summary>
        public HorizontalOptionCategory(string title = "", BaseOption[] options = null, OptionCategory[] subCategories = null, float? gapBetweenThings = null, int? fontSize = null, float titleSizeMultiplier = 2) : base (title, options, subCategories, gapBetweenThings, fontSize, titleSizeMultiplier)
        {
        }

        public override Vector2 CalcSize(int fontSize, float gap)
        {
            var tmpFontSize = this.fontSize == null ? fontSize : this.fontSize.Value;
            GUIBox.ChangeFontSize(tmpFontSize);
            var tmpGap = this.gap == null ? gap : this.gap.Value;

            Vector2 result = Vector2.zero;

            if (title != "")
            {
                result = GUIBox.CalcTextSize(title, (int)(tmpFontSize * titleMulti)) + new Vector2(0, gap * 3);
            }

            if (type == OptionCategoryType.optionHolder)
            {
                foreach (var o in options)
                {
                    if (o.GetHeight() > result.y)
                    {
                        result.y = o.GetHeight();
                    }

                    result.x += o.GetWidth() + gap;
                }
            }
            if (type == OptionCategoryType.subHolder)
            {
                foreach (var s in subCategories)
                {
                    var n = s.CalcSize(tmpFontSize, tmpGap);
                    if (n.y > result.y)
                    {
                        result.y = n.y;
                    }
                    result.x += n.x;
                }
            }

            GUIBox.ChangeFontSize(12);
            return result;
        }

        public override Vector2 updateSubs(Vector2 startCorner, float gap, int fontSize)
        {
            Vector2 updatingCorner = startCorner;
            Vector2 size;
            foreach (OptionCategory category in subCategories)
            {
                size = category.OnGUI(updatingCorner, gap, fontSize);
                updatingCorner += new Vector2(size.x, 0);
            }
            return updatingCorner;
        }

        public override Vector2 updateOptions(Vector2 startCorner, float gap, int fontSize)
        {
            Vector2 updatingCorner = startCorner;
            Vector2 size;
            foreach (BaseOption option in options)
            {
                size = option.Update(updatingCorner, fontSize) + new Vector2(0, gap);
                updatingCorner += new Vector2(size.x + gap, 0);
            }
            return updatingCorner;
        }
    }

    public enum OptionCategoryType
    {
        subHolder,
        optionHolder
    }
}