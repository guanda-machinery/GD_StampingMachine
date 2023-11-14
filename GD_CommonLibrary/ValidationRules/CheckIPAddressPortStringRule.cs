using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GD_CommonLibrary.ValidationRules
{
    public class CheckIPAddressPortStringRule : ValidationRule
    {
        private const string localhostString = "localhost";
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {

                var IPString = value as string;

                if (IPString.Count(x => x == ':') == 1)
                { 
                    if(IPString.Last() == '/')
                    {
                        return new ValidationResult(false, $"IP格式不正確，不可以用'/'當結尾");
                    }



                    var SplitedAddress =  IPString.Split(':');
                    if (SplitedAddress.Count() == 2)   //冗餘驗證，上面IPString.Count已確保只會出現兩個
                    {
                        if (IPAddress.TryParse(SplitedAddress.FirstOrDefault(), out var Parse_IPaddress))
                        {
                            
                        }
                        else if (SplitedAddress.FirstOrDefault().Equals(localhostString, StringComparison.OrdinalIgnoreCase))
                        {

                            //例外情況 字串為localhost(本機端)
                        }
                        else
                        {
                            return new ValidationResult(false, $"IP格式不正確，須為xxx.xxx.xxx.xxx之型別");
                        }



                        //去掉後面的斜線值
                        var ValidPortString = SplitedAddress[1];
                        var ForwardSlashIndex = ValidPortString.IndexOf('/');
                        if (ForwardSlashIndex != -1)
                        {
                            ValidPortString = ValidPortString.Remove(ForwardSlashIndex);
                        }
                        var BackslashSlashIndex = ValidPortString.IndexOf('\\');
                        if (BackslashSlashIndex != -1)
                        {
                            ValidPortString = ValidPortString.Remove(BackslashSlashIndex);
                        }

                        if (int.TryParse(ValidPortString, out var Parse_Port))
                        {
                            if (Parse_Port > 65535)
                            {
                                return new ValidationResult(false, $"端口設定不正確，不可大於65535");
                            }
                            if (Parse_Port < 1)
                            {
                                return new ValidationResult(false, $"端口設定不正確，不可小於1");
                            }
                        }
                        else
                        {
                            return new ValidationResult(false, $"端口格式不正確，須為整數數字");
                        }
                        return ValidationResult.ValidResult;

                    }
                    return new ValidationResult(false, $"切割字串不正確，格式須為xxx.xxx.xxx.xxx:o");
                }
                else
                {
                    if (IPString.Count(x => x == ':') == 0)
                    {
                        return new ValidationResult(false, $"字串格式不正確，須有一分號分隔IPAddress及Port！");
                    }
                    else
                    {
                        if (IPString.Contains("http://"))
                        {
                            return new ValidationResult(false, $"字串格式不正確，通訊地址不是http://開頭");
                        }
                        else
                        {
                            return new ValidationResult(false, $"字串格式不正確，分號只能出現一次！");
                        }
                    }
                }
            }
            else
            {
                throw new Exception("IP驗證僅能檢查字串格式");
            }
        }
    }
}
