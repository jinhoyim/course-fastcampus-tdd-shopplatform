package shopplatform.accounting.test;

import autoparams.CsvAutoSource;
import autoparams.customization.Customization;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.params.ParameterizedTest;
import shopplatform.accounting.Order;
import shopplatform.accounting.OrderView;
import shopplatform.accounting.OrderViewAggregator;

import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.List;

class OrderViewAggregatorTests {

    @ParameterizedTest
    @CsvAutoSource({
            "Pending, 보류",
            "AwaitingPayment, 결제대기",
            "AwaitingShipment, 배송대기",
            "Completed, 완료",
    })
    @Customization(AccountingCustomizer.class)
    void sut_localizes_status(String status,
                              String localizedStatus,
                              OrderViewAggregator sut,
                              Order order) {
        // Arrange
        setStatus(order, status);

        // Act
        Iterable<OrderView> views = sut.aggregateViews(List.of(order));

        // Assert
        ArrayList<OrderView> list = new ArrayList<>();
        views.forEach(list::add);
        Assertions.assertEquals(localizedStatus, list.getFirst().status());
    }

    private static void setStatus(Order order, Object value) {
        try {
            Field field = Order.class.getDeclaredField("status");
            field.setAccessible(true);
            field.set(order, value);
        } catch (NoSuchFieldException | IllegalAccessException e) {
            throw new RuntimeException(e);
        }
    }
}
