import axios from 'axios';
import {
    CreatedPerformanceStrategy,
    CreatedPerformanceStrategyEquation,
    DeletedPerformanceStrategyEquations,
    emptyPerformanceStrategy,
    PerformanceStrategy,
    PerformanceStrategyEquation,
    UpdatedPerformanceStrategy
} from '@/shared/models/iAM/performance';
import {mockPerformanceStrategies} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class PerformanceEditorService {
    /**
     * Gets all performance strategies a user can read/edit
     */
    getPerformanceStrategies(): Promise<PerformanceStrategy[]> {
        return Promise.resolve<PerformanceStrategy[]>(mockPerformanceStrategies);
        // TODO: add axios web service call for performance strategies
    }

    /**
     * Creates a performance strategy
     * @param createdPerformanceStrategy The performance strategy create data
     */
    createPerformanceStrategy(createdPerformanceStrategy: CreatedPerformanceStrategy): Promise<PerformanceStrategy> {
        return Promise.resolve<PerformanceStrategy>({
           ...emptyPerformanceStrategy,
           name: createdPerformanceStrategy.name
        });
        // TODO: add axios web service call for creating performance strategy
    }

    /**
     * Updates a performance strategy
     * @param updatePerformanceStrategy The performance strategy update data
     */
    updatePerformanceStrategy(updatedPerformanceStrategy: UpdatedPerformanceStrategy): Promise<PerformanceStrategy> {
        return Promise.resolve<PerformanceStrategy>({
            id: updatedPerformanceStrategy.id,
            name: updatedPerformanceStrategy.name,
            description: updatedPerformanceStrategy.description,
            performanceStrategyEquations: updatedPerformanceStrategy.performanceStrategyEquations
        });
        // TODO: add axios web service call for updating performance strategy
    }

    /**
     * Creates a performance strategy equation
     * @param createdEquation The performance strategy equation create data
     */
    createEquation(createdEquation: CreatedPerformanceStrategyEquation): Promise<PerformanceStrategyEquation> {
        return Promise.resolve<PerformanceStrategyEquation>({
            ...createdEquation,
            performanceStrategyEquationId: 0,
            equation: '',
            criteria: '',
            shift: false,
            piecewise: false,
            isFunction: false
        });
        // TODO: add axios web service call for creating a performance strategy equation
    }

    /**
     * Updates a performance strategy equation
     * @param updatedEquation The performance strategy equation update data
     */
    updateEquation(updatedEquation: PerformanceStrategyEquation): Promise<PerformanceStrategyEquation> {
        return Promise.resolve<PerformanceStrategyEquation>(updatedEquation);
        // TODO: add axios web service call for updating performance strategy equation
    }

    /**
     * Deletes performance strategy equations
     * @param deletedEquations The performance strategy equations' delete data
     */
    deleteEquations(deletedEquations: DeletedPerformanceStrategyEquations): Promise<DeletedPerformanceStrategyEquations> {
        return Promise.resolve<DeletedPerformanceStrategyEquations>(deletedEquations);
        // TODO: add axios web service call for deleting performance strategy equations
    }
}
