using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GD_CommonLibrary.ValidationRules
{
    public class CheckNumberArrayRule : ValidationRule
    {
        public bool Nullable { get; set; } = true;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (((string)value).Length > 0)
            {
                //用空白或逗號切開
                var ValueArray = ((string)value).Split(' ', ',');

                foreach(var SpiltedValue in ValueArray)
                {
                    if (double.TryParse(SpiltedValue, out var DoubleValue))
                    {
                        if(DoubleValue > 0)
                        {
                            if (int.TryParse(SpiltedValue, out var IntValue))
                            {
                                continue;
                            }
                            else
                            {
                                return new ValidationResult(false, $"數值形式須為整數型!");
                            }
                        }
                        else
                        {
                            return new ValidationResult(false, $"數值需大於零!");
                        }
                    }
                    else
                    {
                        return new ValidationResult(false, $"有非數字的值存在!");
                    }
                }
            }
            else if(!Nullable)
            {
                return new ValidationResult(false, $"需輸入數值!");
            }


            return ValidationResult.ValidResult;
        }
    }
}
