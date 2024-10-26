package shopplatform.accounting.test;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import shopplatform.accounting.Order;
import shopplatform.accounting.OrderView;
import shopplatform.accounting.OrderViewAggregator;
import shopplatform.accounting.ShopReader;

import java.lang.reflect.Field;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

class OrderViewAggregatorTests {

    @ParameterizedTest
    @CsvSource({
            "Pending, 보류",
            "AwaitingPayment, 결제대기",
            "AwaitingShipment, 배송대기",
            "Completed, 완료",
    })
    void sut_localizes_status(String status, String localizedStatus) {
        // Arrange
        ShopReader shopReader = id -> Optional.empty();
        OrderViewAggregator sut = new OrderViewAggregator(shopReader);
        var order = getOrder(status);

        // Act
        Iterable<OrderView> views = sut.aggregateViews(List.of(order));

        // Assert
        ArrayList<OrderView> list = new ArrayList<>();
        views.forEach(list::add);
        Assertions.assertEquals(localizedStatus, list.getFirst().status());
    }

    private static Order getOrder(String status) {
        var order = new Order();
        setField(order, "id", UUID.randomUUID());
        setField(order, "userId", UUID.randomUUID());
        setField(order, "shopId", UUID.randomUUID());
        setField(order, "itemId", UUID.randomUUID());
        setField(order, "price", new BigDecimal(100000));
        setField(order, "status", status);
        setField(order, "placedAtUtc", LocalDateTime.now());
        return order;
    }

    private static void setField(Order order, String name, Object value) {
        try {
            Field field = Order.class.getDeclaredField(name);
            field.setAccessible(true);
            field.set(order, value);
        } catch (NoSuchFieldException | IllegalAccessException e) {
            throw new RuntimeException(e);
        }
    }
}
