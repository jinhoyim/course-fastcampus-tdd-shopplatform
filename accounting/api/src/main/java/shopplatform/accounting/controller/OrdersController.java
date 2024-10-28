package shopplatform.accounting.controller;

import io.swagger.v3.oas.annotations.media.ArraySchema;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import org.springframework.web.bind.annotation.*;
import shopplatform.accounting.Order;
import shopplatform.accounting.OrderReader;
import shopplatform.accounting.OrderView;
import shopplatform.accounting.OrderViewAggregator;
import shopplatform.accounting.controller.query.GetOrdersPlacedIn;

import java.time.LocalDateTime;

@RestController()
@RequestMapping("/api/orders")
public class OrdersController {

    private final OrderReader reader;
    private final OrderViewAggregator aggregator;

    public OrdersController(OrderReader reader, OrderViewAggregator aggreator) {
        this.reader = reader;
        this.aggregator = aggreator;
    }

    @PostMapping("/get-orders-placed-in")
    @ApiResponse(content = {
            @Content(
                    mediaType = "application/json",
                    array = @ArraySchema(schema = @Schema(implementation = OrderView.class))
            )
    })
    public Iterable<OrderView> getOrdersPlacedIn(@RequestBody GetOrdersPlacedIn query) {
        LocalDateTime startInclusive = LocalDateTime.of(query.year(), query.month(), 1, 0, 0);
        LocalDateTime endExclusive = startInclusive.plusMonths(1);
        Iterable<Order> orders = reader.getOrdersPlacedIn(
                query.shopId(),
                startInclusive,
                endExclusive);
        return aggregator.aggregateViews(orders);
    }
}
