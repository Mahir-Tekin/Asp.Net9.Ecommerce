﻿// <auto-generated />
using System;
using Asp.Net9.Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Asp.Net9.Ecommerce.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250702124120_init2")]
    partial class init2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid?>("ParentCategoryId")
                        .HasColumnType("uuid");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("ParentCategoryId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("AverageRating")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(3, 2)
                        .HasColumnType("numeric(3,2)")
                        .HasDefaultValue(0m);

                    b.Property<decimal>("BasePrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("ReviewCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AverageRating");

                    b.HasIndex("BasePrice");

                    b.HasIndex("CategoryId");

                    b.HasIndex("IsActive");

                    b.HasIndex("Name");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.HasIndex("IsActive", "DeletedAt")
                        .HasDatabaseName("IX_Products_IsActive_DeletedAt");

                    b.ToTable("Products", t =>
                        {
                            t.HasCheckConstraint("CK_Products_BasePrice", "\"BasePrice\" > 0");
                        });
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ProductImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AltText")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsMain")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImage");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ProductReview", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("HelpfulVotes")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("UnhelpfulVotes")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("ProductId");

                    b.HasIndex("Rating");

                    b.HasIndex("UserId");

                    b.HasIndex("ProductId", "DeletedAt")
                        .HasDatabaseName("IX_ProductReviews_ProductId_DeletedAt");

                    b.HasIndex("ProductId", "UserId")
                        .IsUnique();

                    b.ToTable("ProductReviews", t =>
                        {
                            t.HasCheckConstraint("CK_ProductReviews_Content", "\"Title\" IS NOT NULL OR \"Comment\" IS NOT NULL");

                            t.HasCheckConstraint("CK_ProductReviews_HelpfulVotes", "\"HelpfulVotes\" >= 0");

                            t.HasCheckConstraint("CK_ProductReviews_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");

                            t.HasCheckConstraint("CK_ProductReviews_UnhelpfulVotes", "\"UnhelpfulVotes\" >= 0");
                        });
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ProductVariant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("MinStockThreshold")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<decimal?>("OldPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<bool>("TrackInventory")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal?>("_price")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)")
                        .HasColumnName("Price");

                    b.Property<int>("_stockQuantity")
                        .HasColumnType("integer")
                        .HasColumnName("StockQuantity");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("ProductId");

                    b.HasIndex("SKU")
                        .IsUnique()
                        .HasFilter("\"DeletedAt\" IS NULL");

                    b.HasIndex("_stockQuantity");

                    b.HasIndex("IsActive", "DeletedAt")
                        .HasDatabaseName("IX_ProductVariants_IsActive_DeletedAt");

                    b.HasIndex("ProductId", "IsActive");

                    b.ToTable("ProductVariants", t =>
                        {
                            t.HasCheckConstraint("CK_ProductVariants_MinStockThreshold", "\"MinStockThreshold\" >= 0");

                            t.HasCheckConstraint("CK_ProductVariants_Price", "\"Price\" IS NULL OR \"Price\" > 0");

                            t.HasCheckConstraint("CK_ProductVariants_StockQuantity", "\"StockQuantity\" >= 0");
                        });
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ReviewVote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ReviewId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("VoteType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReviewId", "UserId")
                        .IsUnique()
                        .HasDatabaseName("IX_ReviewVotes_ReviewId_UserId");

                    b.ToTable("ReviewVotes", (string)null);
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.VariantOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayValue")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<Guid>("VariationTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Value");

                    b.HasIndex("VariationTypeId");

                    b.ToTable("VariantOptions");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.VariationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("VariationTypes");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Orders.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Orders.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("ProductSlug")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<Guid>("ProductVariantId")
                        .HasColumnType("uuid");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<decimal>("UnitPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("VariantName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("CategoryVariationType", b =>
                {
                    b.Property<Guid>("CategoriesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("VariationTypesId")
                        .HasColumnType("uuid");

                    b.HasKey("CategoriesId", "VariationTypesId");

                    b.HasIndex("VariationTypesId");

                    b.ToTable("CategoryVariationTypes", (string)null);
                });

            modelBuilder.Entity("ProductVariantVariantOption", b =>
                {
                    b.Property<Guid>("ProductVariantId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SelectedOptionsId")
                        .HasColumnType("uuid");

                    b.HasKey("ProductVariantId", "SelectedOptionsId");

                    b.HasIndex("SelectedOptionsId");

                    b.ToTable("ProductVariantOptions", (string)null);
                });

            modelBuilder.Entity("ProductVariationType", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("VariantTypesId")
                        .HasColumnType("uuid");

                    b.HasKey("ProductId", "VariantTypesId");

                    b.HasIndex("VariantTypesId");

                    b.ToTable("ProductVariantTypes", (string)null);
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.Category", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.Category", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.Product", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ProductImage", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ProductReview", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.Product", "Product")
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ProductVariant", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.Product", "Product")
                        .WithMany("Variants")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ReviewVote", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.ProductReview", "Review")
                        .WithMany("Votes")
                        .HasForeignKey("ReviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Review");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.VariantOption", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.VariationType", "VariationType")
                        .WithMany("Options")
                        .HasForeignKey("VariationTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("VariationType");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Orders.Order", b =>
                {
                    b.OwnsOne("Asp.Net9.Ecommerce.Domain.Orders.OrderAddress", "ShippingAddress", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uuid");

                            b1.Property<string>("AddressLine")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("AddressTitle")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)");

                            b1.Property<string>("District")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)");

                            b1.Property<string>("Neighborhood")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)");

                            b1.Property<string>("PhoneNumber")
                                .IsRequired()
                                .HasMaxLength(15)
                                .HasColumnType("character varying(15)");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.Navigation("ShippingAddress")
                        .IsRequired();
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Orders.OrderItem", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Orders.Order", null)
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CategoryVariationType", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.VariationType", null)
                        .WithMany()
                        .HasForeignKey("VariationTypesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductVariantVariantOption", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.ProductVariant", null)
                        .WithMany()
                        .HasForeignKey("ProductVariantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.VariantOption", null)
                        .WithMany()
                        .HasForeignKey("SelectedOptionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductVariationType", b =>
                {
                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Asp.Net9.Ecommerce.Domain.Catalog.VariationType", null)
                        .WithMany()
                        .HasForeignKey("VariantTypesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.Category", b =>
                {
                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.Product", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Reviews");

                    b.Navigation("Variants");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.ProductReview", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Catalog.VariationType", b =>
                {
                    b.Navigation("Options");
                });

            modelBuilder.Entity("Asp.Net9.Ecommerce.Domain.Orders.Order", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
