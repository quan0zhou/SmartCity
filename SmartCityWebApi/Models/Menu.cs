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
               Icon="dashboard",
               Title="仪表盘",
               Show= true

             })
             {
                 Name="dashboard",
                 ParentId=0,
                 Id=1,
                 Component="RouteView",
                 Redirect="/dashboard/workplace"
             },
              new Menu (new Meta
             {
               Title="工作台",
               Show= true

             })
             {
                 Name="workplace",
                 ParentId=1,
                 Id=7,
                 Component="Workplace"
             },
             new Menu (new Meta
             {
               Title="分析页",
               Show= true

             })
             {
                 Name="Analysis",
                 ParentId=1,
                 Id=2,
                 Component="Analysis",
                 Path="/dashboard/analysis"
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
