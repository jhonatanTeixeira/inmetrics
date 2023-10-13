CREATE STREAM daily_cash_flow_stream (
  Id STRING,
  UserId STRING,
  Amount DECIMAL(10,2),
  Date TIMESTAMP
) 
WITH (KAFKA_TOPIC = 'transaction-created', VALUE_FORMAT = 'JSON', PARTITIONS=1);

CREATE TABLE IF NOT EXISTS daily_cash_flow_table 
WITH(format='JSON')
    AS SELECT
        UserId,
        SUM(Amount) AS Total,
        CAST(Date as DATE) as Date
    FROM daily_cash_flow_stream
    GROUP BY UserId, CAST(Date as DATE);