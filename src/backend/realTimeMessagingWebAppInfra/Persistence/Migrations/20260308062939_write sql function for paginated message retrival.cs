using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realTimeMessagingWebApp.Migrations
{
    // hand written migration
    public partial class writesqlfunctionforpaginatedmessageretrival : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
                CREATE OR REPLACE FUNCTION GetPaginatedChatHistory(
                    ChatId UUID,
                    StartMessageSequence int,
                    EndMessageSequence int DEFAULT -1,
                    FallBackToMaxInt boolean DEFAULT true
                )
                RETURNS TABLE (
                    ""MessageId"" UUID,
                    ""SenderId"" UUID,
                    ""ChatId"" UUID,
                    ""MessageContent"" text,
                    ""SentAt"" timestamptz,
                    ""IsEdited"" boolean,
                    ""EditedAt"" timestamptz,
                    ""IsDeleted"" boolean,
                    ""DeletedAt"" timestamptz,
                    ""SequenceNumber"" integer
                )
                LANGUAGE plpgsql
                AS $$
                DECLARE
                    maxSeq integer;
                    effectiveStart integer := StartMessageSequence;
                    effectiveEnd integer := EndMessageSequence;
                BEGIN
                    IF StartMessageSequence < 0 THEN
                        RAISE EXCEPTION 'StartMessageSequence cannot be negative';
                    END IF;

                    IF EndMessageSequence < -1 THEN
                        RAISE EXCEPTION 'EndMessageSequence cannot be less than -1, got %', EndMessageSequence;
                    END IF;

                    IF StartMessageSequence > EndMessageSequence AND EndMessageSequence != -1 THEN
                        IF NOT FallBackToMaxInt THEN
                            RAISE EXCEPTION 'StartMessageSequence % is greater than EndMessageSequence %', StartMessageSequence, EndMessageSequence;
                        END IF;
                    END IF;

                    SELECT MAX(""SequenceNumber"") INTO maxSeq
                    FROM ""Messages""
                    WHERE ""Messages"".""ChatId"" = GetPaginatedChatHistory.ChatId;

                    IF effectiveEnd = -1 THEN
                        effectiveEnd := maxSeq;
                    END IF;

                    IF effectiveStart > maxSeq THEN
                        IF FallBackToMaxInt THEN
                            effectiveStart := maxSeq;
                            effectiveEnd := maxSeq;
                        ELSE
                            RAISE EXCEPTION 'StartMessageSequence % exceeds max sequence %', effectiveStart, maxSeq;
                        END IF;
                    END IF;

                    IF effectiveEnd > maxSeq THEN
                        IF FallBackToMaxInt THEN
                            effectiveEnd := maxSeq;
                        ELSE
                            RAISE EXCEPTION 'EndMessageSequence % exceeds max sequence %', effectiveEnd, maxSeq;
                        END IF;
                    END IF;

                    RETURN QUERY
                    SELECT * FROM ""Messages""
                    WHERE ""Messages"".""ChatId"" = GetPaginatedChatHistory.ChatId
                      AND ""SequenceNumber"" >= effectiveStart
                      AND ""SequenceNumber"" <= effectiveEnd;
                END;
                $$;";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS GetPaginatedChatHistory(UUID, int, int, boolean);");
        }
    }
}
