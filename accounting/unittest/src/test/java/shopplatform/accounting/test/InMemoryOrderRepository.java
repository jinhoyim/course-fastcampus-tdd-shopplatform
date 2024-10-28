package shopplatform.accounting.test;

import shopplatform.accounting.Order;
import shopplatform.accounting.OrderReader;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.UUID;

public class InMemoryOrderRepository extends ArrayList<Order> implements OrderReader {
    @Override
    public Iterable<Order> getOrdersPlacedIn(
            UUID shopId,
            LocalDateTime placedAtUtcStartInclusive,
            LocalDateTime placedAtUtcEndExclusive) {
        return stream()
                .filter(x -> x.getShopId().equals(shopId))
                .filter(x -> x.getPlacedAtUtc().compareTo(placedAtUtcStartInclusive) >= 0)
                .filter(x -> x.getPlacedAtUtc().compareTo(placedAtUtcEndExclusive) < 0)
                .toList();
    }
}
