package shopplatform.accounting.test;

import autoparams.customization.Customizer;
import autoparams.generator.ObjectContainer;
import autoparams.generator.ObjectGenerator;
import shopplatform.accounting.ShopReader;

import java.util.Optional;

public class EmptyShopReaderCustomizer implements Customizer {
    @Override
    public ObjectGenerator customize(ObjectGenerator generator) {
        return (objectQuery, resolutionContext) ->
                objectQuery.getType().equals(ShopReader.class)
                        ? new ObjectContainer(getShopReader())
                        : generator.generate(objectQuery, resolutionContext);
    }

    private ShopReader getShopReader() {
        return id -> Optional.empty();
    }
}
