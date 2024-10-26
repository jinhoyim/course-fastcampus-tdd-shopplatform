package shopplatform.accounting;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;

import java.net.http.HttpClient;

@SpringBootApplication
public class AccountingApplication {
    public static void main(String[] args) {
        SpringApplication.run(AccountingApplication.class, args);
    }

    @Bean
    public ShopReader shopReader(@Value("${sellers.host}") String host) {
        return new HttpShopReader(HttpClient.newBuilder().build(), host);
    }

    @Bean
    public OrderViewAggregator aggregator(ShopReader shopReader) {
        return new OrderViewAggregator(shopReader);
    }
}