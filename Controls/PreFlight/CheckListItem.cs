using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace MissionPlanner.Controls.PreFlight
{
    public class CheckListItem
    {
        // the default source for the value we are going to get
        public static object defaultsrc { get; set; }

        // for nested conditions
        public CheckListItem Child = null;

        // colour to use for text when the condition is true
        internal Color _TrueColor { get; set; }
        public string TrueColor { get { return _TrueColor.Name; } set { _TrueColor = Color.FromName(value); } }

        // colour to use for the text when the condition is false
        internal Color _FalseColor { get; set; }
        public string FalseColor { get { return _FalseColor.Name; } set { _FalseColor = Color.FromName(value); } }

        // the description
        public string Description { get; set; }

        // the text to display
        public string Text { get; set; }
        
        // the value to trigger the change.
        public double TriggerValue { get; set; }

        // name of the property we are using
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                if (defaultsrc != null) SetField(value);
            }
        }

        string _name = "";

        // the property we are watching for our value
        System.Reflection.PropertyInfo Item { get; set; }

        /// <summary>
        /// condition we are checking for
        /// </summary>
        public Conditional ConditionType { get; set; }

        public enum Conditional
        {
            NONE = 0,
            LT,
            LTEQ,
            EQ,
            GT,
            GTEQ,
            NEQ
        }

        /// <summary>
        /// Returns the formated string to pass to the speech engine
        /// </summary>
        /// <returns></returns>
        public string DisplayText()
        {
            if (Name == "PARAM")
            {
                var valuep = HandleParam();

                var answer = Convert.ToDouble(valuep);

                return
                    Text.Replace("{trigger}", TriggerValue.ToString("0.##"))
                        .Replace("{value}", answer.ToString("0.##"));
            }

            if (Item == null)
                return "";

            var value = GetValueObject;

            if (Item.PropertyType.Name == "Single" || Item.PropertyType.Name == "Double")
            {
                var answer = Convert.ToDouble(value);

                return
                    Text.Replace("{trigger}", TriggerValue.ToString("0.##"))
                        .Replace("{value}", answer.ToString("0.##"))
                        .Replace("{name}", Item.Name);
            }
            else
            {
                return Text.Replace("{trigger}", TriggerValue.ToString("0.##"))
                        .Replace("{value}", value.ToString())
                        .Replace("{name}", Item.Name);
            }
        }

        /// <summary>
        /// Get the current value
        /// </summary>
        public double GetValue
        {
            get
            {
                if (defaultsrc == null)
                    throw new ArgumentNullException("src");

                if (Name == "PARAM")
                {
                    return HandleParam();
                }

                if (Item == null)
                    return 0;

                if (Item.PropertyType.Name == "String")
                    return 0;

                if (Item.PropertyType.Name == "DateTime")
                    return 0;

                try
                {
                    return (double) Convert.ChangeType(Item.GetValue(defaultsrc, null), typeof (double));
                }
                catch (InvalidCastException)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Get the current value
        /// </summary>
        public object GetValueObject
        {
            get
            {
                if (defaultsrc == null)
                    throw new ArgumentNullException("src");

                if (Name == "PARAM")
                {
                    return HandleParam();
                }

                if (Item == null)
                    throw new ArgumentNullException("Item");

                return Item.GetValue(defaultsrc, null);
            }
        }

        double HandleParam()
        {
            if (Name == "PARAM")
            {
                var match = Regex.Match(Description, @"\{(.+)\}");
                if (match.Success)
                {
                    var paramname = match.Groups[1].Value;

                    if (MainV2.comPort.MAV.param.ContainsKey(paramname))
                    {
                        var answer = MainV2.comPort.MAV.param[paramname].Value;
                        return answer;
                    }
                }
            }
            return 0;
        }

        public void SetField(string name)
        {
            if (defaultsrc == null)
                throw new ArgumentNullException("src");

            if (name == "")
                return;

            Type test = defaultsrc.GetType();

            foreach (var field in test.GetProperties())
            {
                if (field.Name == name)
                {
                    Item = field;
                    Name = name;
                    return;
                }
            }

            if (name == "PARAM")
            {
                Item = null;
                Name = name;
                return;
            }

            throw new MissingFieldException("No such name");
        }

        /// <summary>
        /// Return the list of options that are avaliable
        /// </summary>
        public List<string> GetOptions()
        {
            if (defaultsrc == null)
                throw new ArgumentNullException("src");

            List<string> answer = new List<string>();

            Type test = defaultsrc.GetType();

            foreach (var field in test.GetProperties())
            {
                // field.Name has the field's name.
                object fieldValue;
                TypeCode typeCode;
                try
                {
                    fieldValue = field.GetValue(defaultsrc, null); // Get value

                    if (fieldValue == null)
                        continue;

                    // Get the TypeCode enumeration. Multiple types get mapped to a common typecode.
                    typeCode = Type.GetTypeCode(fieldValue.GetType());
                }
                catch
                {
                    continue;
                }

                answer.Add(field.Name);
            }

            answer.Add("PARAM");

            return answer;
        }

        public bool checkCond(CheckListItem item)
        {
            // if there is a child go recursive
            if (item.Child != null)
            {
                if (item.CheckValue() && checkCond(item.Child))
                    return true;
            }
            else
            {
                // is no child then simple check
                if (item.CheckValue())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// return true on match, and uses repeat time to prevent spamming
        /// </summary>
        /// <returns></returns>
        bool CheckValue()
        {
            switch (ConditionType)
            {
                case Conditional.EQ:
                    if (GetValue == TriggerValue)
                        return true;
                    break;
                case Conditional.GT:
                    if (GetValue > TriggerValue)
                        return true;
                    break;
                case Conditional.GTEQ:
                    if (GetValue >= TriggerValue)
                        return true;
                    break;
                case Conditional.LT:
                    if (GetValue < TriggerValue)
                        return true;
                    break;
                case Conditional.LTEQ:
                    if (GetValue <= TriggerValue)
                        return true;
                    break;
                case Conditional.NEQ:
                    if (GetValue != TriggerValue)
                        return true;
                    break;
                case Conditional.NONE:

                    break;
            }

            return false;
        }
    }
}
