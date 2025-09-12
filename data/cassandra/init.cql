CREATE KEYSPACE IF NOT EXISTS ecommerce WITH replication = 
  {'class': 'SimpleStrategy', 'replication_factor': 1};

CREATE TABLE ecommerce.orders (
    order_id text PRIMARY KEY,
    customer text,
    amount double,
    status text
);
