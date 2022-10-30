# WebAPI - [DELETE] 405 Method Not Allowed

如果使用的是IIS 7.0或更高版本部署。这个问题与WebDAV扩展模块有关。这将在发布或删除请求时出现。请尝试在web.config中进行以下设置 

```xml
<system.webServer>
 <modules>
   <remove name="WebDAVModule" />
 </modules>
 <handlers>
   <remove name="WebDAV" />
 </handlers>
</system.webServer>
```

> https://stackoverflow.com/questions/20445653/asp-net-web-api-http-delete-405-method-not-allowed
