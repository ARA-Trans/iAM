import {keys} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';

/**
 * Converts a mongo model to an acceptable vue model
 * @param mongoModel Mongo model to convert
 */
export const convertFromMongoToVue = (mongoModel: any) => {
    const convertedModel: any = {
        ...mongoModel,
        id: mongoModel._id
    };

    keys(convertedModel).forEach((prop) => {
        if (hasValue(convertedModel[prop])) {
            if (Array.isArray(convertedModel[prop])) {
                convertedModel[prop] = convertedModel[prop]
                    .map((model: any) => {
                        if (typeof model === 'object') {
                            return convertFromMongoToVue(model);
                        } else {
                            return model;
                        }
                    });
            } else if (typeof convertedModel[prop] === 'object') {
                convertedModel[prop] = convertFromMongoToVue(convertedModel[prop]);
            }
        }
    });

    delete convertedModel._id;
    delete convertedModel.__v;

    return convertedModel;
};

/**
 * Converts a vue model into an acceptable mongo model
 * @param vueModel Vue model to convert
 */
export const convertFromVueToMongo = (vueModel: any) => {
    const convertedModel: any = {
        ...vueModel,
        _id: vueModel.id
    };

    keys(convertedModel).forEach((prop) => {
        if (hasValue(convertedModel[prop])) {
            if (Array.isArray(convertedModel[prop])) {
                convertedModel[prop] = convertedModel[prop]
                    .map((model: any) => {
                        if (typeof model === 'object') {
                            return convertFromVueToMongo(model);
                        } else {
                            return model;
                        }
                    });
            } else if (typeof convertedModel[prop] === 'object') {
                convertedModel[prop] = convertFromVueToMongo(convertedModel[prop]);
            }
        }
    });

    delete convertedModel.id;

    return convertedModel;
};