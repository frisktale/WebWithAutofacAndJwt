# Demo介绍
基于Asp.NET Core 6，使用Autofac替换原生IOC容器，实现了AOP，添加了JWT授权和鉴权  
学习[JwtAuthDemo](https://github.com/dotnet-labs/JwtAuthDemo)后自己写的Demo，仅实现了基础功能，原项目还有刷新Token功能，日后再学习

加入Identity，使用dbfirst需要在 `WebWithAutofacAndJwt.Web` 文件夹下，输入  
1. `dotnet ef migrations add NewMigrationv1.0.1 --project ..\WebWithAutofacAndJwt.Migrations --context AppMigrations`
1. `dotnet ef database update --project ..\WebWithAutofacAndJwt.Migrations --context AppMigrations`

命令1生成迁移，并将迁移创建在`WebWithAutofacAndJwt.Migrations`项目重  
命令2将修改应用到数据库

代码仅作示例，很多细枝末节没去处理（比如应该新建一个继承`IdentityRole<long>`的Role类）。
测试前先访问`InitRole`接口，在数据库中创建Role。  
注册时根据用户名判断Role，用户名为admin则权限为admin，其他为basic。  
数据库使用的是PostgreSQL  
数据库字符串在用户机密中配置如下
```json
{
  "ConnectionStrings": {
    "PgSqlConnection": ""
  }
}
```