﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SmartCityWebApi.Infrastructure;

#nullable disable

namespace SmartCityWebApi.Migrations
{
    [DbContext(typeof(SmartCityContext))]
    partial class SmartCityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SmartCityWebApi.Domain.CustSpace", b =>
                {
                    b.Property<long>("SpaceId")
                        .HasColumnType("bigint");

                    b.Property<string>("ContactName")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("ContactPhone")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp");

                    b.Property<long>("CreateUser")
                        .HasColumnType("bigint");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasColumnType("varchar(500)");

                    b.Property<string>("SpaceAddress")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("SpaceName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("SpaceType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("timestamp");

                    b.Property<long>("UpdateUser")
                        .HasColumnType("bigint");

                    b.HasKey("SpaceId");

                    b.ToTable("custSpace");
                });

            modelBuilder.Entity("SmartCityWebApi.Domain.CustSpaceSetting", b =>
                {
                    b.Property<long>("CustId")
                        .HasColumnType("bigint");

                    b.Property<string>("AppID")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("AppKey")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("AppSecret")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<int>("BookableDays")
                        .HasColumnType("integer");

                    b.Property<string>("CertificatePrivateKey")
                        .IsRequired()
                        .HasColumnType("varchar(2048)");

                    b.Property<string>("CertificateSerialNumber")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp");

                    b.Property<long>("CreateUser")
                        .HasColumnType("bigint");

                    b.Property<float>("DirectRefundPeriod")
                        .HasColumnType("real");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time");

                    b.Property<string>("MchID")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("ReservationTitle")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<int>("SettableDays")
                        .HasColumnType("integer");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time");

                    b.Property<string>("SubMchID")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<float>("TimePeriod")
                        .HasColumnType("real");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("timestamp");

                    b.Property<long>("UpdateUser")
                        .HasColumnType("bigint");

                    b.HasKey("CustId");

                    b.ToTable("custSpaceSetting");
                });

            modelBuilder.Entity("SmartCityWebApi.Domain.Order", b =>
                {
                    b.Property<long>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamp");

                    b.Property<decimal>("Money")
                        .HasColumnType("numeric");

                    b.Property<string>("OpenId")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("OrderNo")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("PayTime")
                        .HasColumnType("timestamp");

                    b.Property<string>("PaymentNo")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("RefundOptUser")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("RefundRemark")
                        .IsRequired()
                        .HasColumnType("varchar(500)");

                    b.Property<DateTime?>("RefundTime")
                        .HasColumnType("timestamp");

                    b.Property<DateOnly>("ReservationDate")
                        .HasColumnType("date");

                    b.Property<long>("ReservationId")
                        .HasColumnType("bigint");

                    b.Property<string>("ReservationUserName")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("ReservationUserPhone")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<long>("SpaceId")
                        .HasColumnType("bigint");

                    b.Property<string>("SpaceName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("SpaceType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("timestamp");

                    b.HasKey("OrderId");

                    b.ToTable("order");
                });

            modelBuilder.Entity("SmartCityWebApi.Domain.Reservation", b =>
                {
                    b.Property<long>("ReservationId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamp");

                    b.Property<bool>("IsBooked")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Money")
                        .HasColumnType("numeric");

                    b.Property<DateOnly>("ReservationDate")
                        .HasColumnType("date");

                    b.Property<int>("ReservationStatus")
                        .HasColumnType("integer");

                    b.Property<long>("SpaceId")
                        .HasColumnType("bigint");

                    b.Property<string>("SpaceName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("SpaceType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp");

                    b.HasKey("ReservationId");

                    b.ToTable("reservation");
                });

            modelBuilder.Entity("SmartCityWebApi.Domain.User", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("ContactPhone")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp");

                    b.Property<long>("CreateUser")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasColumnType("varchar(500)");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("timestamp");

                    b.Property<long>("UpdateUser")
                        .HasColumnType("bigint");

                    b.Property<string>("UserAccount")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("UserAccountPwd")
                        .IsRequired()
                        .HasColumnType("varchar(500)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("UserId");

                    b.ToTable("user");
                });

            modelBuilder.Entity("SmartCityWebApi.Domain.UserPermission", b =>
                {
                    b.Property<long>("UserPermissionId")
                        .HasColumnType("bigint");

                    b.Property<int>("PageId")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("UserPermissionId");

                    b.ToTable("userPermission");
                });
#pragma warning restore 612, 618
        }
    }
}
