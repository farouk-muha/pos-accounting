using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace POSAccounting.Utils
{

    public class EmptyValid
    {
        public static string GetErrorMessage(object fieldValue)
        {
            string errorMessage = string.Empty;
            if (fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
                errorMessage = ConstUserMsg.RequiredField;
            return errorMessage;
        }
    }

    public class MyMaxStringRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);

            if (value.ToString().Count() >= 5000)
                return new ValidationResult(false, ConstUserMsg.LittersTooLong + 255);

            return ValidationResult.ValidResult;
        }
    }

    public class MyStringRule255 : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
       {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);

            if (value.ToString().Count() >= 255)
                return new ValidationResult(false, ConstUserMsg.LittersTooLong + 255);

            return ValidationResult.ValidResult;
        }
    }

    public class MyStringNullableRule255 : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(true, "");

            if (value.ToString().Count() >= 255)
                return new ValidationResult(false, ConstUserMsg.LittersTooLong + 255);

            return ValidationResult.ValidResult;
        }
    }
    public class MyRuleEmail : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                string error = EmptyValid.GetErrorMessage(value);
                if (!string.IsNullOrEmpty(error))
                    return new ValidationResult(false, error);

                try
                {
                }
                catch (FormatException ex)
                {
                    return new ValidationResult(false, ConstUserMsg.EmailNotValid);
                }

                if (value.ToString().Count() >= 320)
                    return new ValidationResult(false, ConstUserMsg.LittersTooLong + 320);

                return ValidationResult.ValidResult;
            }

        }

    public class MyStringRule15 : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);

            if (value.ToString().Count() >= 15)
                return new ValidationResult(false, ConstUserMsg.LittersTooLong + 15);

            return ValidationResult.ValidResult;
        }
    }


    public class MyStringNullableRule15 : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(true, "");

            if (value.ToString().Count() >= 15)
                return new ValidationResult(false, ConstUserMsg.LittersTooLong + 15);

            return ValidationResult.ValidResult;
        }
    }

    public class MyDecimalRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);
            decimal num = 0;
            try
            {
                if (((string)value).Length > 0)
                    num = decimal.Parse((String)value);
                if (num >= ConstRuleValue.MaxNum)
                    throw new Exception();
            }
            catch (Exception e)
            {
                return new ValidationResult(false, ConstUserMsg.NumIs0);
            }
            return ValidationResult.ValidResult;
        }
    }

    public class MyDecimalNotZeroRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);
            decimal num = 0;

            try
            {
                if (((string)value).Length > 0)
                    num = decimal.Parse((String)value);
                if (num >= ConstRuleValue.MaxNum )
                {
                    return new ValidationResult(false, ConstUserMsg.NumTooLong);
                }
                if (num == 0)
                    return new ValidationResult(false, ConstUserMsg.NumIs0);
            }
            catch (Exception e)
            {
                return ValidationResult.ValidResult;
            }
            return ValidationResult.ValidResult;
        }
    }

    public class MyIntRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);
            int num = 0;
            try
            {
                if (((string)value).Length > 0)
                    num = int.Parse((String)value);
                if (num < 0)
                    throw new Exception();
            }
            catch (Exception e)
            {
                return new ValidationResult(false, ConstUserMsg.NumIs0);
            }
            return ValidationResult.ValidResult;
        }
    }

    public class MyNumNotZeroRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);
            try
            {
                int num = 0;
                if (((string)value).Length > 0)
                    num = int.Parse((String)value);
                if (num <= 0)
                {
                    return new ValidationResult(false, ConstUserMsg.NumIs0);
                }
                return ValidationResult.ValidResult;
            }
            catch (Exception e)
            {
                return ValidationResult.ValidResult;
            }
        }
    }

    public class MyPercentRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);

            try
            {
                int num = 0;
                if (((string)value).Length > 0)
                    num = int.Parse((String)value);
                if (num < 0 || num > 100)
                {
                    return new ValidationResult(false, ConstUserMsg.PercentOnly);
                }
                return ValidationResult.ValidResult;
            }
            catch (Exception e)
            {
                return ValidationResult.ValidResult;
            }
        }
    }

    public class MyTimeRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string error = EmptyValid.GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);
            try
            {
                byte num = 0;
                if (((string)value).Length > 0)
                    num = byte.Parse((String)value);
                if (num <= 0 || num > 24)
                {
                    return new ValidationResult(false, "Not Valid Time");
                }
                return ValidationResult.ValidResult;
            }
            catch (Exception e)
            {
                return ValidationResult.ValidResult;
            }
        }
    }

    public class ConstRuleValue
    {
        public static decimal MaxNum { get; } = 999999999999;
    }
}
