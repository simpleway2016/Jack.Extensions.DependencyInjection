# Jack.Extensions.DependencyInjection
```
asp.net core中：

startup.cs
using Jack.Extensions.DependencyInjection;

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.SupportController();
            return services.BuildJackServiceProvider();
        }
        
        
需要注入的类：
[DependencyInjection( DependencyInjectionMode.Transient , typeof(IUser))]
class UserInfo : IUser
{
    ...
}

[DependencyInjection]
class Company
{
    ...
}

//使用依赖注入
[Route("api/[controller]")]
[ApiController]
class MyController : ControllerBase
{
    IUser User {get;set;} //这里会被注入
    
    [DependencyInjection]
    Company _Company;      //这个字段因为标注了[DependencyInjection]，也会被注入
    
    Company _Company2;   //这里不被注入
}
```
