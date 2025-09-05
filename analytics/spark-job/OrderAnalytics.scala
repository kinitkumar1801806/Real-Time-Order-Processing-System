import org.apache.spark.sql.SparkSession

object OrderAnalytics{
    def main(args: Array[String]):Unit={
        val spark=SparkSession.builder.appName("OrderAnalytics").getOrCreate()
        val orders=spark.read.json("hdfs://namenode:9000/orders/")
        orders.groupBy("customer").sum("amount").show()
        spark.stop()
    }
}