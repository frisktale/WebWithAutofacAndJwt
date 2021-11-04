# Demo介绍
基于Asp.NET Core 6，使用Autofac替换原生IOC容器，实现了AOP，添加了JWT授权和鉴权  
学习[JwtAuthDemo](https://github.com/dotnet-labs/JwtAuthDemo)后自己写的Demo，仅实现了基础功能，原项目还有刷新Token功能，日后再学习

加入Identity，使用dbfirst需要在 `WebWithAutofacAndJwt.Web` 文件夹下，输入  
1. `dotnet ef migrations add NewMigrationv1.0.1 --project ..\WebWithAutofacAndJwt.Migrations --context AppMigrations`
1. `dotnet ef database update --project ..\WebWithAutofacAndJwt.Migrations --context AppMigrations`

命令1生成迁移，并将迁移创建在`WebWithAutofacAndJwt.Migrations`项目重  
命令2将修改应用到数据库