using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.BusinessLogic.Interfaces;
using FH.ParcelLogistics.DataAccess.Interfaces;
using FH.ParcelLogistics.DataAccess.Sql;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace FH.ParcelLogistics.BusinessLogic;

public class WarehouseValidator : AbstractValidator<Warehouse>
{
    public WarehouseValidator(){
        RuleFor(hopType => hopType.HopType).NotNull();
        RuleFor(code => code.Code).NotNull().Matches(@"^[A-Z]{4}\d{1,4}$");
        RuleFor(description => description.Description).NotNull().Matches(@"^[A-Za-zÄÖÜß][A-Za-zÄÖÜß\d -]*$");
        RuleFor(processingDelayMins => processingDelayMins.ProcessingDelayMins).NotNull();
        RuleFor(locationName => locationName.LocationName).NotNull();
        RuleFor(locationCoordinates => locationCoordinates.LocationCoordinates).NotNull();
        RuleFor(level => level.Level).NotNull();
        RuleFor(nextHops => nextHops.NextHops).NotNull();
    }
}

public class WarehouseCodeValidator : AbstractValidator<string>
{
    public WarehouseCodeValidator(){
        RuleFor(code => code).NotNull().Matches(@"^[A-Z]{4}\d{1,4}$");
    }
}

public class WarehouseLogic : IWarehouseLogic
{
    private readonly WarehouseValidator _warehouseValidator = new WarehouseValidator();
    private readonly WarehouseCodeValidator _warehouseCodeValidator = new WarehouseCodeValidator();
    private readonly IHopRepository _hopRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<IWarehouseLogic> _logger;

    public WarehouseLogic(IHopRepository hopRepository, IMapper mapper, ILogger<IWarehouseLogic> logger){
        _hopRepository = hopRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Warehouse ExportWarehouses(){
        _logger.LogDebug($"ExportWarehouses");

        try{        
            return _mapper.Map<BusinessLogic.Entities.Warehouse>(_hopRepository.Export()); 
        } catch (DALNotFoundException e){
            _logger.LogError($"Export Warehouses: Root warehouse not found");
            throw new BLNotFoundException($"Root warehouse not found", e);
        }
    }

    public Hop GetWarehouse(string code){
        _logger.LogDebug($"GetWarehouse: [code:{code}]");
        // Validate warehouse
        if(!_warehouseCodeValidator.Validate(code).IsValid){
            _logger.LogError($"GetWarehouse: [code:{code}] - Invalid warehouse code");
            throw new BLValidationException("The operation failed due to an error.");
        }

        try {
            return _mapper.Map<BusinessLogic.Entities.Hop>(_hopRepository.GetByCode(code));
        } catch (Exception e) {
            _logger.LogError($"GetWarehouse: [code:{code}] - Error getting warehouse by code");
            throw new BLException($"Error getting warehouse by code", e);
        }
    }

    public void ImportWarehouses(Warehouse warehouse){
        _logger.LogDebug($"ImportWarehouses: [warehouse:{warehouse}]");
        // Validate warehouse
        if(!_warehouseValidator.Validate(warehouse).IsValid){
            _logger.LogError($"ImportWarehouses: [warehouse:{warehouse}] - Invalid warehouse");
            throw new BLValidationException("The operation failed due to an error.");
        }

        try{
            _hopRepository.Import(_mapper.Map<DataAccess.Entities.Warehouse>(warehouse)); 
        } catch (Exception e){
            _logger.LogError($"ImportWarehouses: [warehouse:{warehouse}] - Error importing warehouse");
            throw new BLException($"Error importing warehouse", e);
        }
    
        _logger.LogDebug($"ImportWarehouses: [warehouse:{warehouse}] - Successfully imported warehouse");
    }
}
