using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoTags.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddShareColumnAndStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Share",
                table: "SoTags",
                type: "decimal(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            // Create stored procedure to calculate and update share percentages
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE sp_UpdateTagShares
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @TotalCount BIGINT;
                    
                    -- Calculate total count from all tags
                    SELECT @TotalCount = SUM(CAST([Count] AS BIGINT))
                    FROM [SoTags];
                    
                    -- Prevent division by zero
                    IF @TotalCount = 0
                        SET @TotalCount = 1;
                    
                    -- Update Share for all tags
                    UPDATE [SoTags]
                    SET [Share] = CAST(([Count] * 100.0) / @TotalCount AS NUMERIC(10, 4));
                    
                END;
            ");

            // Execute stored procedure to populate initial share values
            migrationBuilder.Sql("EXEC sp_UpdateTagShares;");

            // Create trigger to recalculate shares when tags are inserted/updated
            migrationBuilder.Sql(@"
                CREATE OR ALTER TRIGGER tr_SoTags_RecalculateShares
                ON [SoTags]
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    EXEC sp_UpdateTagShares;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop trigger
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS tr_SoTags_RecalculateShares;");

            // Drop stored procedure
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateTagShares;");

            // Drop Share column
            migrationBuilder.DropColumn(
                name: "Share",
                table: "SoTags");
        }
    }
}
