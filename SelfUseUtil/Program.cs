// See https://aka.ms/new-console-template for more information
using SelfUseUtil.Helper;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Policy;

int times = 10000000;
DynamicSample dynamicSample = new DynamicSample();
var addMethod = typeof(DynamicSample).GetMethod("Add");

Stopwatch watch1 = Stopwatch.StartNew();
int result = 0;
for (int i = 0; i < times; i++)
{
    result = (int)addMethod.Invoke(dynamicSample, new object[] { 1, 2 });
}
watch1.Stop();
Console.WriteLine(string.Format("正常的反射耗时：{0}毫秒", watch1.ElapsedMilliseconds));
//Console.WriteLine("正常反射的结果：" + result);
/****************************************************************************************************/

dynamic dynamicSample2 = new DynamicSample();
int result2 = 0;
watch1.Restart();
for (int i = 0; i < times; i++)
{
    result2 = dynamicSample2.Add(1, 2);
}
watch1.Stop();
Console.WriteLine(string.Format("Dynamic的反射耗时：{0}毫秒", watch1.ElapsedMilliseconds));
//Console.WriteLine("Dynamic反射的结果：" + result2);
/****************************************************************************************************/

DynamicSample reflectSamplebetter = new DynamicSample();
var addMethod2 = typeof(DynamicSample).GetMethod("Add");
var delg = (Func<DynamicSample, int, int, int>)Delegate.CreateDelegate(typeof(Func<DynamicSample, int, int, int>), addMethod2);
int result3 = 0;
watch1.Restart();
for (int i = 0; i < times; i++)
{
    result3 = delg(reflectSamplebetter, 1, 2);
}
watch1.Stop();
Console.WriteLine(string.Format("优化的反射耗时：{0}毫秒", watch1.ElapsedMilliseconds));
//Console.WriteLine("优化的反射结果：" + result3);
/****************************************************************************************************/

//Console.Read();


class DynamicSample
{
    public string Name { get; set; }
    public int Add(int a, int b)
    {
        return a + b;
    }
}
