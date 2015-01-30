# ATM
Another Type Mapper (DI, IoC  name it as you want)

###Example 1
The simplest way:
```C#
var m = ATMMain.Create();
m.Register(mapper =>
{
    mapper.Map<TestInterface1>().To<TestInterface1Impl1>();
});
var o = m.Get<TestInterface1>();
```

###Example 2
Without generics it looks like this:
```C#
var m = ATMMain.Create();
m.Register(mapper =>
{
    mapper.Map(typeof(TestInterface1)).To(typeof(TestInterface1Impl1));
});
var o = m.Get(typeof(TestInterface1));
```

###Example 3
You can even do this:
```C#
var m = ATMMain.Create();
m.Register(mapper =>
{
    mapper.Map<string>().To<string>().WithParameters(new { c = 'a', count = 5 });
});
var o = m.Get<string>();
```
After that variable 'o' contains this string: "aaaaa"
######Note: The properties of the 'parameter object' should have exactly the same name as the parameters of the constructor of the class. If you don't provide every parameter for the constructor the framework will try to resolve the missing ones.

###Example 4
If you want a singleton, you can do this:
```C#
var m = ATMMain.Create();
m.Register(mapper =>
{
    mapper.Map<TestInterface1>().To<TestInterface1Impl1>().AsSingleton();
});
var o1 = m.Get<TestInterface1>();
var o2 = m.Get<TestInterface1>();
```
Obviously, o1 and o2 will contain the same object.
