package shopplatform.accounting.test;

import autoparams.customization.CompositeCustomizer;
import autoparams.processor.InstanceFieldWriter;
import shopplatform.accounting.Order;

public class AccountingCustomizer extends CompositeCustomizer {
    public AccountingCustomizer() {
        super(new InstanceFieldWriter(Order.class), new EmptyShopReaderCustomizer());
    }
}
