using System;
using System.Globalization;
using System.Windows.Controls;

namespace GD_CommonControlLibrary.ValidationRules
{
    /// <summary>
    /// 驗證是否為數字
    /// </summary>
    public class CheckNumberRule : ValidationRule
    {
        /// <summary>
        /// 設定最大值
        /// </summary>
        public double? NumberMax { get; set; }
        /// <summary>
        /// 設定最小值
        /// </summary>
        public double? NumberMin { get; set; }
        /// <summary>
        /// 是否為整數型
        /// </summary>
        public bool IsINTValidate { get; set; }

        /// <summary>
        /// 欄位是否可為空
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 若數值為特定值則直接通過
        /// </summary>
        public double? SpecificValue { get; set; }

        /// <summary>
        /// 驗證<paramref name="value"/>值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            //先檢查參數
            if (NumberMax is double && NumberMin is double)
            {
                if (NumberMax < NumberMin)
                    throw new Exception("驗證值設定錯誤，最大值不可小於最小值");
            }

            if (!string.IsNullOrEmpty((string)value))
            {
                if (((string)value).EndsWith("."))
                    return new ValidationResult(false, $"結尾不可為小數點");



                if (double.TryParse((string)value, out var DoubleValue))
                {
                    if (IsINTValidate is true)
                    {
                        if (int.TryParse((string)value, out var IntValue))
                        {

                        }
                        else
                        {
                            return new ValidationResult(false, $"須為整數數字!");
                        }
                    }

                    if (SpecificValue != null)
                        if (DoubleValue == SpecificValue)
                            return ValidationResult.ValidResult;

                    if (NumberMax != null)
                    {
                        if (DoubleValue > NumberMax)
                        {
                            return new ValidationResult(false, $"數字不可大於{NumberMax}!");
                        }
                    }

                    if (NumberMin != null)
                    {
                        if (DoubleValue < NumberMin)
                        {
                            return new ValidationResult(false, $"數字不可小於{NumberMin}!");
                        }
                    }


                }
                else
                {
                    return new ValidationResult(false, $"不可有數字以外的文字");
                }
            }
            else
            {
                if (!IsNullable)
                    return new ValidationResult(false, $"欄位不可為空!");
            }

            return ValidationResult.ValidResult;
        }

    }

    public class CheckEmptyRule : ValidationRule
    {
        /// <inheritdoc/>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (((string)value).Length == 0)
                return new ValidationResult(false, $"欄位不可為空!");
            else
            {
                //不可以是空白
                if (string.IsNullOrWhiteSpace((string)value))
                {
                    return new ValidationResult(false, $"欄位不可為空白!");
                }
            }
            return ValidationResult.ValidResult;
        }

    }
}
