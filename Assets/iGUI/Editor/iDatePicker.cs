using System;
using System.Collections;
using System.Collections.Generic;
using InnovaFramework.iGUI;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iDatePickerPopup: iWindow
    {
        public static iDatePickerPopup window;

        public int               fontSize;
        public int               minYear;
        public int               maxYear;
        public bool              isSelected;
        public bool              useStartEndMode;
        public DateTime          firstSelect;
        public DateTime          secondSelect;
        public iDatePicker       picker;
        public Action<DateTime?, DateTime?> callback;

        public static iDatePickerPopup Show(Rect rect, Action<DateTime?, DateTime?> callback, int minYear, int maxYear, DateTime firstSelect, DateTime? secondSelect = null, bool useStartEndMode = false, int fontSize = 12)
        {
            window                 = GetWindow<iDatePickerPopup>();
            window.titleContent    = new GUIContent("Date Picker");
            window.rect            = new Rect(0, 0, rect.width, rect.height - 6);
            window.maxSize         = window.rect.size;
            window.minSize         = window.maxSize;
            window.position        = rect;
            window.callback        = callback;
            window.isSelected      = false;
            window.firstSelect     = firstSelect;
            window.secondSelect    = secondSelect == null ? firstSelect : secondSelect.Value;
            window.minYear         = minYear;
            window.maxYear         = maxYear;
            window.fontSize        = fontSize;
            window.useStartEndMode = useStartEndMode;
            window.ShowUtility();

            return window;
        }


        private void OnGUI()
        {
            base.Render();
        }


        public override void OnDisable()
        {
            if (isSelected) return;
            callback?.Invoke(null, null);
        }


        protected override void OnInitializeUI()
        {
            picker                 = new iDatePicker();
            picker.size            = this.rect.size + new Vector2(0, 6);
            picker.Selected        = firstSelect;
            picker.Selected2       = secondSelect;
            picker.fontSize        = fontSize;
            picker.useStartEndMode = useStartEndMode;
            picker.SetYearsRange(minYear, maxYear);
            picker.RelativePosition(iRelativePosition.CENTER_X_OF, this);
            picker.RelativePosition(iRelativePosition.TOP_IN     , this, 0);
            picker.OnChanged = (sender) =>
            {
                isSelected = true;
                Close();
                callback?.Invoke(picker.Selected, picker.Selected2);
            };
            AddChild(picker);
        }


        public void Update()
        {
            if (picker == null) return;
            picker.fontSize = fontSize;
            picker.UpdateRelative();
        }
    }


    internal enum DatePackerState
    {
        PICK_1,
        PICK_2
    }


    public class iDatePicker : iObject
    {
        private iBox             background;
        private iBox             header;
        private iBox             dayBG;
        private iBox             footer;
        private iBox[]           dayOfWeek;
        private iButton          btnMonthNext;
        private iButton          btnMonthPrev;
        private iButton          btnToday;
        private iDropDown        ddMonth;
        private iDropDown        ddYears;
        private DateTime         displayDate;
        private List<iButton>    allDays;
        private iDatePickerPopup currentPopup = null;

        // Properties
        public bool            useAsPopup;
        public bool            useStartEndMode;
        public Action<iObject> OnChanged;

        private int              minYears;
        private int              maxYears;
        private DateTime         _selected;
        private DateTime         _selected2;

        private GUIStyle        backgroundStyle;
        private GUIStyle        highlightStyle;
        private GUIStyle        selectedStyle;
        private DatePackerState pickerState;

        public  DateTime Selected       { get { return _selected;  } set { _selected  = value; if (!useStartEndMode) _selected2 = value; ReGenerateDate(_selected.Month, _selected.Year); } }
        public  DateTime Selected2      { get { return _selected2; } set { _selected2 = value; } }
        public  bool     isPopupShowing { get { return currentPopup != null; } }


        public iDatePicker()
        {
            header        = new iBox();
            footer        = new iBox();
            dayBG         = new iBox();
            background    = new iBox();
            dayOfWeek     = new iBox[7];
            btnMonthNext  = new iButton();
            btnMonthPrev  = new iButton();
            btnToday      = new iButton();
            ddMonth       = new iDropDown();
            ddYears       = new iDropDown();

            backgroundStyle = new GUIStyle();
            highlightStyle  = new GUIStyle();
            selectedStyle   = new GUIStyle();

            for (int i = 0; i < dayOfWeek.Length; i++)
            {
                dayOfWeek[i] = new iBox();
            }

            allDays               = new List<iButton>();
            this._backgroundColor = new Color(0.28f, 0.28f, 0.28f);
            this._contentColor    = new Color(0.18f, 0.18f, 0.18f);
            this.useAsPopup       = false;
            this.useStartEndMode  = false;
            this.size             = new Vector2(200, 250);
            this.pickerState      = DatePackerState.PICK_1;

            SetTheme(backgroundColor, new Color(0.24f, 0.30f, 0.36f), new Color(0.17f, 0.36f, 0.52f), new Color(0.9f, 0.9f, 0.9f), new Color(1, 0.8f, 0));

            // Month Year
            string[] month    = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            ddMonth.SetOptions(month);
            ddMonth.OnChanged = (sender) => ReGenerateDate(ddMonth.index + 1, displayDate.Year);
            ddYears.OnChanged = (sender) => ReGenerateDate(displayDate.Month, (int)ddYears.selectedObject);

            int currentYear = DateTime.Now.Year;
            SetYearsRange(currentYear - 70, currentYear + 70);
            Selected = DateTime.Now;
        }


        public void SetYearsRange(int min, int max)
        {
            ddYears.ClearOption();
            for (int i = min; i <= max; i++)
            {
                ddYears.AddOption(i.ToString(), i);
            }

            minYears = min;
            maxYears = max;
        }


        public void Show(Action<DateTime?, DateTime?> callback, DateTime? firstDate = null, DateTime? secondDate = null)
        {
            if (!useAsPopup) return;
            if (currentPopup != null)
            {
                currentPopup.Focus();
                return;
            }

            Rect rect = new Rect(this.rect);
            rect.y -= 24;

            currentPopup = iDatePickerPopup.Show(EditorGUIUtility.GUIToScreenRect(rect), (s1, s2) => 
            {
                currentPopup = null;

                if (s1 != null) Selected  = s1.Value;
                if (s2 != null) Selected2 = s2.Value;

                callback?.Invoke(s1, s2);

            }, minYears, maxYears, firstDate == null ? DateTime.Now : firstDate.Value, secondDate, useStartEndMode, fontSize);
        }


        public override void Start()
        {
            dayBG.LoadWhiteTexture();
            dayBG.backgroundColor = this.contentColor;

            header.LoadWhiteTexture();
            header.backgroundColor = this.contentColor;

            footer.LoadWhiteTexture();
            footer.backgroundColor = this.contentColor;

            background.LoadWhiteTexture();
            background.backgroundColor = this.backgroundColor;

            btnMonthNext.LoadClickableTexture("mat-2-icon.chevron_right");
            btnMonthNext.OnClicked = (sender) => 
            {
                displayDate = displayDate.AddMonths(1);
                ReGenerateDate(displayDate.Month, displayDate.Year);
            };

            btnMonthPrev.LoadClickableTexture("mat-2-icon.chevron_left");
            btnMonthPrev.OnClicked = (sender) =>
            {
                displayDate = displayDate.AddMonths(-1);
                ReGenerateDate(displayDate.Month, displayDate.Year);
            };

            btnToday.text = "Today";
            btnToday.LoadClickableTexture(iGUIUtility.GetSolidTextureColor(contentColor));
            btnToday.style.SetTextColorForAll(Color.white);
            btnToday.style.alignment = TextAnchor.MiddleCenter;
            btnToday.OnClicked = (sender) => 
            {
                pickerState = DatePackerState.PICK_1;
                Selected2 = DateTime.Now;
                Selected = DateTime.Now;
            };

            // Day Of Week
            string[] days = new string[] { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };
            for (int i = 0; i < days.Length; i++)
            {
                iBox box            = dayOfWeek[i];
                box.text            = days[i];
                box.size.x          = this.size.x / 7f;
                box.backgroundColor = contentColor;
                box.LoadWhiteTexture();
                if (i == 0)
                {
                    box.contentColor = new Color(1f, 0.3f, 0.3f);
                }
            }

            OnRelativePositionChanged();
            ReGenerateDate(Selected.Month, Selected.Year);
        }


        public override void Render()
        {
            if (useAsPopup) return;

            base.Render();
            background.Render();
            header.Render();
            dayBG.Render();
            btnMonthNext.Render();
            btnMonthPrev.Render();
            ddMonth.Render();
            ddYears.Render();
            footer.Render();
            btnToday.Render();

            for(int i = 0; i < dayOfWeek.Length; i++)
            {
                dayOfWeek[i].Render();
            }

            for(int i = 0; i < allDays.Count; i++)
            {
                allDays[i].Render();
            }
        }


        protected override void OnRelativePositionChanged()
        {
            // Layout
            background.RelativePosition(iRelativePosition.TOP   , this);
            background.RelativePosition(iRelativePosition.LEFT  , this);
            background.RelativeSize(() => background.size = this.size );

            header.RelativePosition(iRelativePosition.TOP        , background);
            header.RelativePosition(iRelativePosition.CENTER_X_OF, background);
            header.RelativeSize( () => new Vector2(background.width, iGUIUtility.SplitSizeByPercentage01(background.height, 0.15f)) );

            footer.size.x = header.size.x;
            footer.size.y = iGUIUtility.singleLineHeight + 8;
            footer.RelativePosition(iRelativePosition.BOTTOM_IN  , background, 0);
            footer.RelativePosition(iRelativePosition.CENTER_X_OF, background, 0);

            btnMonthNext.RelativePosition(iRelativePosition.RIGHT_IN   , header, 4);
            btnMonthNext.RelativePosition(iRelativePosition.CENTER_Y_OF, header);
            btnMonthNext.size.x = header.size.y.space();
            btnMonthNext.size.y = header.size.y.space();

            btnMonthPrev.RelativePosition(iRelativePosition.LEFT_IN    , header, 4);
            btnMonthPrev.RelativePosition(iRelativePosition.CENTER_Y_OF, header);
            btnMonthPrev.size.x = header.size.y.space();
            btnMonthPrev.size.y = header.size.y.space();

            float availableSize    = header.width - btnMonthNext.widthEx * 2f;
            float[] sizePercentage = iGUIUtility.SplitSizeByPercentage(availableSize, 0.6f, 0.4f);

            ddMonth.size.x   = sizePercentage[0];
            ddMonth.size.y   = btnMonthPrev.height;
            ddMonth.fontSize = fontSize;
            ddMonth.RelativePosition(iRelativePosition.RIGHT_OF   , btnMonthPrev, 4);
            ddMonth.RelativePosition(iRelativePosition.CENTER_Y_OF, header);

            ddYears.size.x   = sizePercentage[1];
            ddYears.size.y   = btnMonthPrev.height;
            ddYears.fontSize = fontSize;
            ddYears.RelativePosition(iRelativePosition.LEFT_OF    , btnMonthNext, 4);
            ddYears.RelativePosition(iRelativePosition.CENTER_Y_OF, header);

            // Day of week
            float avaiH = iGUIUtility.HeightBetween2Objects(header, footer, 0);
            float sizeY = avaiH / 7f;

            for (int i = 0; i < dayOfWeek.Length; i++)
            {
                iBox box     = dayOfWeek[i];
                box.size.y   = sizeY;
                box.fontSize = fontSize;
                box.RelativePosition(iRelativePosition.BOTTOM_OF, header, 0);
                if (i == 0)
                {
                    box.RelativePosition(iRelativePosition.LEFT, this);
                }
                else
                {
                    box.RelativePosition(iRelativePosition.RIGHT_OF, dayOfWeek[i - 1], 0);
                }
            }

            dayBG.size.x = this.width;
            dayBG.size.y = sizeY;
            dayBG.RelativePosition(iRelativePosition.BOTTOM_OF  , header, 0);
            dayBG.RelativePosition(iRelativePosition.CENTER_X_OF, header, 0);

            // Footer
            btnToday.text   = "Today";
            btnToday.size.y = footer.height;
            btnToday.RelativePosition(iRelativePosition.RIGHT_IN   , footer, 0);
            btnToday.RelativePosition(iRelativePosition.CENTER_Y_OF, footer);
        }


        private void ReGenerateDate(int month, int year)
        {
            DateTime inputDT = new DateTime(year, month, 1);
            DateTime startDT = inputDT.AddDays(-(int)inputDT.DayOfWeek);

            float avaiH = iGUIUtility.HeightBetween2Objects(header, footer, 0);
            float sizeY = avaiH / 7f;

            int col = 7;
            int row = 6;

            iObject refTop  = dayOfWeek[0];
            iObject refLeft = null;

            for(int i = 0; i < col * row; i++)
            {
                DateTime curDT = startDT.AddDays(i);

                iButton btn    = null;
                if (i >= allDays.Count)
                {
                    btn = new iButton();
                    btn.useEventCallback = true;
                    btn.OnMouseOver += (sender, evt) =>
                    {
                        if (!useStartEndMode)                      return;
                        if (pickerState != DatePackerState.PICK_2) return;
                        UpdateStartEndColor((DateTime)sender.attechment);
                    };
                    allDays.Add(btn);
                }
                else
                {
                    btn = allDays[i];
                }

                btn.text       = curDT.Day.ToString();
                btn.size.x     = this.width / 7f;
                btn.size.y     = sizeY;
                btn.attechment = curDT;
                btn.fontSize   = fontSize;

                btn.RelativePosition(iRelativePosition.BOTTOM_OF, refTop, 0);
                if (refLeft == null) { btn.RelativePosition(iRelativePosition.LEFT_IN, this, 0); } else { btn.RelativePosition(iRelativePosition.RIGHT_OF, refLeft, 0); }

                refLeft = btn;
                if ( (i + 1) % col == 0)
                {
                    refTop  = btn;
                    refLeft = null;
                }

                btn.OnClicked = (sender) =>
                {
                    DateTime senderDateTime = (DateTime)sender.attechment;
                    if (!useStartEndMode)
                    {
                        Selected  = senderDateTime;
                        OnChanged?.Invoke(this);
                    }
                    else
                    {
                        if (pickerState == DatePackerState.PICK_1)
                        {
                            pickerState = DatePackerState.PICK_2;
                            Selected2   = senderDateTime;
                            Selected    = senderDateTime;
                        }
                        else
                        {
                            if (DateTime.Compare(senderDateTime,Selected) < 0) return;

                            pickerState = DatePackerState.PICK_1;
                            Selected2   = senderDateTime;
                            UpdateStartEndColor(Selected2);
                            OnChanged?.Invoke(this);
                        }
                    }
                    Event.current.Use();
                };

            }

            displayDate   = new DateTime(year, month, 1);
            ddMonth.index = month - 1;
            ddYears.index = ddYears.GetIndexByOptionName(year.ToString());

            UpdateStartEndColor(Selected2);
            GUI.changed = true;
        }


        private void UpdateStartEndColor(DateTime currentMouse)
        {
            for(int i = 0; i < allDays.Count; i++)
            {
                DateTime dt = (DateTime)allDays[i].attechment;

                int res = DateTime.Compare(dt, Selected);
                int end = DateTime.Compare(dt, currentMouse);

                if (dt.CompareDate(Selected) || dt.CompareDate(Selected2) || dt.CompareDate(currentMouse))
                {
                    allDays[i].SetClickableTexture(selectedStyle);
                    allDays[i].style.SetTextColorForAll(selectedStyle.normal.textColor);
                    allDays[i].style.fontStyle = FontStyle.Bold;
                }
                else
                {
                    allDays[i].SetClickableTexture( (res > 0 && end < 0) ? highlightStyle : backgroundStyle);
                    allDays[i].style.SetTextColorForAll( dt.Month == displayDate.Month ? backgroundStyle.normal.textColor : Color.gray);
                    allDays[i].style.fontStyle = FontStyle.Normal;
                }

                allDays[i].style.alignment = TextAnchor.MiddleCenter;
            }

        }


        private int GetNumberOfDays(int month, int year)
        {
            return DateTime.DaysInMonth(year, month);
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();
        }


        public override void Dispose()
        {
            if (currentPopup != null)
            {
                currentPopup.isSelected = true;
                currentPopup.Close();
            }
        }


        protected override void OnContentColorChanged()
        {
            dayBG.backgroundColor  = this.contentColor;
            header.backgroundColor = this.contentColor;
            footer.backgroundColor = this.contentColor;
            GUI.changed = true;
        }


        public void SetTheme(Color? normalColor, Color? highlightColor, Color? selectedColor, Color? textNormal, Color? textSelected)
        {
            if (textNormal != null)
            {
                backgroundStyle.normal.textColor = textNormal.Value;
            }

            if (normalColor != null)
            {
                backgroundStyle.LoadClickableTexture(iGUIUtility.GetSolidTextureColor(normalColor.Value));
                background.backgroundColor = normalColor.Value;
            }

            if (highlightColor != null)
            {
                highlightStyle.LoadClickableTexture(iGUIUtility.GetSolidTextureColor(highlightColor.Value));
            }

            if (selectedColor != null)
            {
                selectedStyle.LoadClickableTexture(iGUIUtility.GetSolidTextureColor(selectedColor.Value));
            }

            if (textSelected != null)
            {
                selectedStyle.normal.textColor = textSelected.Value;
            }

            try { UpdateStartEndColor(Selected2); } catch {} 
        }
    }

}