// See https://aka.ms/new-console-template for more information
using SelfUseUtil.Helper;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Policy;


object num = null;
Console.WriteLine(Convert.ToDecimal(num ?? 0));
Console.WriteLine((decimal)(num ?? 0m));


