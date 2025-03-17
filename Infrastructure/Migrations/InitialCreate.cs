
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SurveyApp.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalyticsData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TotalSurveys = table.Column<int>(nullable: false),
                    TotalResponses = table.Column<int>(nullable: false),
                    AverageCompletionRate = table.Column<double>(nullable: false),
                    QuestionTypeDistribution = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandName = table.Column<string>(maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(maxLength: 200, nullable: false),
                    ContactPhone = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    AcquiredServices = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBaseItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Category = table.Column<string>(maxLength: 100, nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBaseItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(maxLength: 1000, nullable: false),
                    CustomerName = table.Column<string>(maxLength: 200, nullable: false),
                    CustomerEmail = table.Column<string>(maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    Category = table.Column<string>(maxLength: 100, nullable: true),
                    Priority = table.Column<string>(nullable: true),
                    IsAnonymous = table.Column<bool>(nullable: false),
                    Response = table.Column<string>(maxLength: 1000, nullable: true),
                    ResponseDate = table.Column<DateTime>(nullable: true),
                    SimilarSuggestions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suggestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Responses = table.Column<int>(nullable: false),
                    CompletionRate = table.Column<int>(nullable: false),
                    DeliveryConfig_Type = table.Column<string>(nullable: true),
                    DeliveryConfig_EmailAddresses = table.Column<string>(nullable: true),
                    DeliveryConfig_Schedule_Frequency = table.Column<string>(nullable: true),
                    DeliveryConfig_Schedule_DayOfMonth = table.Column<int>(nullable: true),
                    DeliveryConfig_Schedule_DayOfWeek = table.Column<int>(nullable: true),
                    DeliveryConfig_Schedule_Time = table.Column<string>(nullable: true),
                    DeliveryConfig_Schedule_StartDate = table.Column<DateTime>(nullable: true),
                    DeliveryConfig_Trigger_Type = table.Column<string>(nullable: true),
                    DeliveryConfig_Trigger_DelayHours = table.Column<int>(nullable: true),
                    DeliveryConfig_Trigger_SendAutomatically = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResponseTrends",
                columns: table => new
                {
                    Date = table.Column<string>(maxLength: 20, nullable: false),
                    Responses = table.Column<int>(nullable: false),
                    AnalyticsDataId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseTrends", x => x.Date);
                    table.ForeignKey(
                        name: "FK_ResponseTrends_AnalyticsData_AnalyticsDataId",
                        column: x => x.AnalyticsDataId,
                        principalTable: "AnalyticsData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrowthMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Period = table.Column<string>(maxLength: 20, nullable: false),
                    Revenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserCount = table.Column<int>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrowthMetrics_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 500, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<string>(nullable: false),
                    Required = table.Column<bool>(nullable: false),
                    Options = table.Column<string>(nullable: true),
                    SurveyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrowthMetrics_CustomerId",
                table: "GrowthMetrics",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SurveyId",
                table: "Questions",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseTrends_AnalyticsDataId",
                table: "ResponseTrends",
                column: "AnalyticsDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrowthMetrics");

            migrationBuilder.DropTable(
                name: "KnowledgeBaseItems");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "ResponseTrends");

            migrationBuilder.DropTable(
                name: "Suggestions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "AnalyticsData");
        }
    }
}
