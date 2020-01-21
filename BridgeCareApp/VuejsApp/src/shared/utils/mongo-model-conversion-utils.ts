import {keys} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';

/**
 * Converts a mongo model to an acceptable vue model
 * @param mongoModel Mongo model to convert
 */
export const convertFromMongoToVue = (mongoModel: any) => {
    var convertedModel: any = {
        ...mongoModel,
        id: mongoModel._id
    };

    delete convertedModel._id;
    delete convertedModel.__v;

    return performRecursiveModelConversions(convertedModel, convertFromMongoToVue);
};

/**
 * Converts a vue model into an acceptable mongo model
 * @param vueModel Vue model to convert
 */
export const convertFromVueToMongo = (vueModel: any) => {
    var convertedModel: any = {
        ...vueModel,
        _id: vueModel.id
    };

    delete convertedModel.id;

    return performRecursiveModelConversions(convertedModel, convertFromVueToMongo);
};

/**
 * Performs a recursive model conversion on any objects in the given object
 * @param model The model to convert
 * @param conversionFunction The function to use to convert models within the model
 */
const performRecursiveModelConversions = (model: any, objectConversionFunction: any) => {
    if (Array.isArray(model)) {
        model = model.map((subModel) => {
            if (typeof subModel === 'object') {
                return objectConversionFunction(subModel);
            }
            return subModel;
        });
    } else if (typeof model === 'object') {
        keys(model).forEach((prop) => {
            if (!hasValue(model[prop])) {
                return;
            }
            if (Array.isArray(model[prop])) {
                model[prop] = performRecursiveModelConversions(model[prop], objectConversionFunction);
            } else if (typeof model[prop] === 'object') {
                model[prop] = objectConversionFunction(model[prop]);
            }
        });
    }
    return model;
};