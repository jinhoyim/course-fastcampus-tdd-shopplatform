package shopplatform.accounting.test.api.orders.get_orders_placed_in;

import autoparams.CsvAutoSource;
import autoparams.customization.Customization;
import org.junit.jupiter.params.ParameterizedTest;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.context.TestConfiguration;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Import;
import org.springframework.context.annotation.Primary;
import shopplatform.accounting.Order;
import shopplatform.accounting.OrderReader;
import shopplatform.accounting.OrderView;
import shopplatform.accounting.ShopReader;
import shopplatform.accounting.controller.query.GetOrdersPlacedIn;
import shopplatform.accounting.test.AccountingCustomizer;
import shopplatform.accounting.test.InMemoryOrderRepository;

import java.lang.reflect.Field;
import java.time.LocalDateTime;
import java.util.Arrays;
import java.util.Optional;

import static org.assertj.core.api.Assertions.assertThat;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@Import(Post_specs.TestDoubles.class)
public class Post_specs {

    @Autowired
    private TestRestTemplate client;

    @Autowired
    private InMemoryOrderRepository orderRepository;

    @TestConfiguration
    public static class TestDoubles {
        @Bean
        public InMemoryOrderRepository inMemoryOrderRepository() {
            return new InMemoryOrderRepository();
        }

        @Bean
        @Primary
        public OrderReader orderReaderDouble(InMemoryOrderRepository repository) {
            return repository;
        }

        @Bean
        @Primary
        public ShopReader shopReaderDouble() {
            return id -> Optional.empty();
        }
    }

    @ParameterizedTest
    @CsvAutoSource({
            "2024-09-30T23:59:59, 2024, 10, true",
            "2024-09-01T00:00:00, 2024, 9, true",
            "2024-08-31T14:59:59, 2024, 9, false",
            "2024-10-31T14:59:59, 2024, 10, true",
    })
    @Customization(AccountingCustomizer.class)
    void sut_correctly_globalizes_time_window(
            String placedAtUtcSource,
            int year,
            int month,
            boolean selected,
            Order order
    ) {
        setPlacedAtUtc(order, LocalDateTime.parse(placedAtUtcSource));
        orderRepository.add(order);

        String path = "/api/orders/get-orders-placed-in";
        var query = new GetOrdersPlacedIn(order.getShopId(), year, month);
        var views = client.postForObject(path, query, OrderView[].class);

        boolean actual = Arrays.stream(views).anyMatch(v -> v.id().equals(order.getId()));
        assertThat(actual).isEqualTo(selected);
    }

    private void setPlacedAtUtc(Order order, LocalDateTime value) {
        try {
            Field field = Order.class.getDeclaredField("placedAtUtc");
            field.setAccessible(true);
            field.set(order, value);

        } catch (NoSuchFieldException | IllegalAccessException e) {
            throw new RuntimeException(e);
        }
    }
}
