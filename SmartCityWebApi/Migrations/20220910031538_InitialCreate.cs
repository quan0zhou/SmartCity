using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCityWebApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "custSpace",
                columns: table => new
                {
                    SpaceId = table.Column<long>(type: "bigint", nullable: false),
                    SpaceName = table.Column<string>(type: "varchar(50)", nullable: false),
                    SpaceAddress = table.Column<string>(type: "varchar(200)", nullable: false),
                    ContactName = table.Column<string>(type: "varchar(20)", nullable: false),
                    ContactPhone = table.Column<string>(type: "varchar(20)", nullable: false),
                    Remark = table.Column<string>(type: "varchar(500)", nullable: false),
                    SpaceType = table.Column<int>(type: "integer", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custSpace", x => x.SpaceId);
                });

            migrationBuilder.CreateTable(
                name: "custSpaceSetting",
                columns: table => new
                {
                    CustId = table.Column<long>(type: "bigint", nullable: false),
                    ReservationTitle = table.Column<string>(type: "varchar(20)", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    TimePeriod = table.Column<float>(type: "real", nullable: false),
                    SettableDays = table.Column<int>(type: "integer", nullable: false),
                    BookableDays = table.Column<int>(type: "integer", nullable: false),
                    DirectRefundPeriod = table.Column<float>(type: "real", nullable: false),
                    AppID = table.Column<string>(type: "varchar(200)", nullable: false),
                    MchID = table.Column<string>(type: "varchar(200)", nullable: false),
                    SubMchID = table.Column<string>(type: "varchar(200)", nullable: false),
                    AppKey = table.Column<string>(type: "varchar(200)", nullable: false),
                    AppSecret = table.Column<string>(type: "varchar(200)", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custSpaceSetting", x => x.CustId);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    OrderNo = table.Column<string>(type: "varchar(50)", nullable: false),
                    PaymentNo = table.Column<string>(type: "varchar(50)", nullable: false),
                    OpenId = table.Column<string>(type: "varchar(200)", nullable: false),
                    ReservationUserName = table.Column<string>(type: "varchar(20)", nullable: false),
                    ReservationUserPhone = table.Column<string>(type: "varchar(20)", nullable: false),
                    ReservationId = table.Column<long>(type: "bigint", nullable: false),
                    ReservationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SpaceId = table.Column<long>(type: "bigint", nullable: false),
                    SpaceType = table.Column<int>(type: "integer", nullable: false),
                    SpaceName = table.Column<string>(type: "varchar(50)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    OrderStatus = table.Column<int>(type: "integer", nullable: false),
                    RefundTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    PayTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    RefundRemark = table.Column<string>(type: "varchar(500)", nullable: false),
                    RefundOptUser = table.Column<string>(type: "varchar(100)", nullable: false),
                    Money = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "reservation",
                columns: table => new
                {
                    ReservationId = table.Column<long>(type: "bigint", nullable: false),
                    ReservationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SpaceId = table.Column<long>(type: "bigint", nullable: false),
                    SpaceType = table.Column<int>(type: "integer", nullable: false),
                    SpaceName = table.Column<string>(type: "varchar(50)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ReservationStatus = table.Column<int>(type: "integer", nullable: false),
                    Money = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation", x => x.ReservationId);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "varchar(50)", nullable: false),
                    UserAccount = table.Column<string>(type: "varchar(20)", nullable: false),
                    UserAccountPwd = table.Column<string>(type: "varchar(500)", nullable: false),
                    ContactPhone = table.Column<string>(type: "varchar(20)", nullable: false),
                    Remark = table.Column<string>(type: "varchar(500)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "userPermission",
                columns: table => new
                {
                    UserPermissionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userPermission", x => x.UserPermissionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "custSpace");

            migrationBuilder.DropTable(
                name: "custSpaceSetting");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "userPermission");
        }
    }
}
