using GD_CommonLibrary.Extensions;
using System.Globalization;
using System.Windows.Controls;

namespace GD_CommonLibrary.ValidationRules
{
    public enum PathTypeEnum
    {
        Path,
        FileName
    }

    public class CheckStringIsPathOrFileNameRule : ValidationRule
    {
        public PathTypeEnum PathType { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string ValueString)
            {
                if (string.IsNullOrWhiteSpace(ValueString))
                {
                    return new ValidationResult(false, "檔案路徑不可為空白");
                }

                if (PathType == PathTypeEnum.Path)
                {
                    if (!ValueString.IsPathRooted())
                    {
                        return new ValidationResult(false, "路徑須包含根目錄");
                    }

                    if (ValueString.IsContain_illegalPathChars(out var illegal))
                    {
                        return new ValidationResult(false, $"路徑包含不合法的字元[{illegal.ExpandToString()}]");
                    }
                }
                if (PathType == PathTypeEnum.FileName)
                {
                    if (ValueString.IsLegalFileName(out var illegal))
                    {
                        return new ValidationResult(false, $"檔案名稱包含不合法的字元[{illegal.ExpandToString()}]");
                    }
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
