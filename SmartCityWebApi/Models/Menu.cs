using System.ComponentModel;

namespace SmartCityWebApi.Models
{
    public class Menu
    {
        public Menu(Meta meta)
        {
            Meta = meta;
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int ParentId { get; set; }

        public string? Component { get; set; }

        public string? Redirect { get; set; }

        public string? Path { get; set; }

        public Meta Meta { get; set; }

        public static List<Menu> MenuList = new List<Menu>()
        {
             new Menu (new Meta
             {
               Icon="home",
               Title="首页",
               HideChildren=true

             })
             {
                 Name="dashboard",
                 ParentId=0,
                 Id=1000,
                 Component="Workplace",         
             },
              new Menu (new Meta
             {
               Icon="form",
               Title="预订管理"

             })
             {
                 Name="form",
                 ParentId=0,
                 Id=2000,
                 Component="RouteView",
               
             },
                new Menu (new Meta
             {
               Title="预订"

             })
             {
                 Name="basicform",
                 ParentId=2000,
                 Id=2001,
                 Component="BasicForm"
             },
              new Menu (new Meta
             {
               Title="订单记录"

             })
             {
                 Name="stepForm",
                 ParentId=2000,
                 Id=2002,
                 Component="StepForm"
             },
              new Menu (new Meta
             {
               Icon="table",
               Title="后台管理"

             })
             {
                 Name="list",
                 ParentId=0,
                 Id=3000,
                 Component="RouteView",

             },  
             new Menu (new Meta
             {
               Title="场地管理"

             })
             {
                 Name="park",
                 ParentId=3000,
                 Id=3001,
                 Component="TableList"
             },
              new Menu (new Meta
             {
               Title="配置管理"

             })
             {
                 Name="setting",
                 ParentId=3000,
                 Id=3002,
                 Component="StandardList"
             },
                new Menu (new Meta
             {
               Icon="user",
               Title="用户管理",
               HiddenHeaderContent=true,
               HideHeader=true,
               HideChildren=false,

             })
             {
                 Name="userManage",
                 ParentId=0,
                 Id=4000,
                 Component="RouteView",

             },
              new Menu (new Meta
             {
               Title="用户列表",
               HiddenHeaderContent=true,
               HideHeader=true,
               HideChildren=true,
              

             })
             {
                 Name="user",
                 ParentId=4000,
                 Id=4001,
                 Component="ProfileBasic"
             },
        };

    }
    public class Meta
    {
        public string Title { get; set; } = string.Empty;

        public string? Icon { get; set; }

        public bool? Show { get; set; }

        public bool? HiddenHeaderContent { get; set; }

        public bool? HideHeader { get; set; }

        public bool? HideChildren { get; set; }
    }
}
