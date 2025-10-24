using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    [XmlRoot(ElementName = "FulfillmentExternalRequests")]
    public class FulfillmentExternalRequests
    {
        [XmlElement("FulfillmentExternalRequest")]
        public List<FulfillmentExternalRequest> FulfillmentExternalRequest { get; set; } = new();
    }

    public class FulfillmentExternalRequest
    {
        public string ClientID { get; set; }
        public string FedExID { get; set; }
        public string TransactionDate { get; set; }
        public string IsAddTransaction { get; set; }
        public string FileName { get; set; }
        public string InsuredType { get; set; }

        public PrimaryInfo PrimaryInfo { get; set; }
        public Registrants Registrants { get; set; }
        public AdditionalInsureds AdditionalInsureds { get; set; }
        public Vehicles Vehicles { get; set; }
        public Terminals Terminals { get; set; }
        public AdditionalInterests AdditionalInterests { get; set; }
        public Policies Policies { get; set; }
    }

    public class PrimaryInfo
    {
        public ISPInformation ISPInformation { get; set; }
    }

    public class ISPInformation
    {
        public string CustomerSeqNumber { get; set; }
        public string BusinessName { get; set; }
        public string BusNameChgEffDte { get; set; }
        public Address Address { get; set; }
        public string TaxIDNo { get; set; }
        public string TaxIDChangeEffDate { get; set; }
        public string PrincipleOfficer { get; set; }
        public string PrincipleOfficerChgEffDate { get; set; }
        public string PrincipleOfficerTitle { get; set; }
        public string PrincipleOfficerDateofBirth { get; set; }
        public string OwnershipPercentage { get; set; }
        public string BusinessType { get; set; }
        public string BusinessTypeChgEffDate { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        public string Risk_BureauID { get; set; }
        public string Email { get; set; }
    }

    public class Address
    {
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string AddressChgEffDte { get; set; }
    }

    public class Registrants
    {
        [XmlElement("Registrant")]
        public List<Registrant> Registrant { get; set; } = new();
    }

    public class Registrant
    {
        public BusinessInfo BusinessInfo { get; set; }
    }

    public class BusinessInfo
    {
        public string CustomerSeqNumber { get; set; }
        public string BusinessName { get; set; }
        public string BusNameChgEffDate { get; set; }
        public Address Address { get; set; }
        public string TaxID { get; set; }
        public string TaxIdChgEffDate { get; set; }
    }

    public class AdditionalInsureds
    {
        [XmlElement("AdditionalInsured")]
        public List<AdditionalInsured> AdditionalInsured { get; set; } = new();
    }

    public class AdditionalInsured
    {
        public string AdditionalInsured_SequenceNo { get; set; }
        public string BusinessName { get; set; }
        public Address Address { get; set; }
    }

    public class Vehicles
    {
        [XmlElement("VehicleInfo")]
        public List<VehicleInfo> VehicleInfo { get; set; } = new();
    }

    public class VehicleInfo
    {
        public string Vehicle_SequenceNo { get; set; }
        public string VIN { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public decimal? StatedValue { get; set; }
        public string ValueChgEffDte { get; set; }
        public string StateVehiclePlated { get; set; }
        public string PlateStateChgEffDte { get; set; }
        public string Company { get; set; }
        public string ContractID { get; set; }
        public string ActiveStatus { get; set; }
        public string IsEnrolledSupplemental { get; set; }
        public string GarageStatePickupAndDelivery { get; set; }
    }

    public class Terminals
    {
        [XmlElement("TerminalInfo")]
        public List<TerminalInfo> TerminalInfo { get; set; } = new();
    }

    public class TerminalInfo
    {
        public string TerminalNumber { get; set; }
        public Address TerminalAddress { get; set; }
    }

    public class AdditionalInterests
    {
        [XmlElement("AdditionalInterest")]
        public List<AdditionalInterest> AdditionalInterest { get; set; } = new();
    }

    public class AdditionalInterest
    {
        public string AdditionalInterest_SequenceNo { get; set; }
        public string Name { get; set; }
        public string NameChgEffDte { get; set; }
        public string AdditionalInterestType { get; set; }
        public string AdditionalInterestTypeChgDate { get; set; }
        public Address Address { get; set; }
    }

    public class Policies
    {
        [XmlElement("Policy")]
        public List<Policy> Policy { get; set; } = new();
    }

    public class Policy
    {
        public string PolicyNum { get; set; }
        public string PolEffective { get; set; }
        public string PolExpire { get; set; }
        public string AdditionalInsured_SequenceNo { get; set; }
        public string UWCompany { get; set; }
        public string EmployerLiabilityLimit { get; set; }

        [XmlElement("Coverage")]
        public List<Coverage> Coverage { get; set; } = new();

        public InsurePay InsurePay { get; set; }
    }

    public class Coverage
    {
        public string CoverageCode { get; set; }
        public string CoverageDescription { get; set; }
        public decimal? CommissionPercent { get; set; }

        public EPLI EPLI { get; set; }
        public NTL NTL { get; set; }
        public PhysicalDamage PhysicalDamage { get; set; }
        public NTLExcess NTLExcess { get; set; }
        public WorkComp WorkComp { get; set; }
    }

    public class EPLI
    {
        public decimal? EdgewaterCommissionPercent { get; set; }
        public string RetroActiveDate { get; set; }
        public decimal? AggregateLimitAmount { get; set; }
        public decimal? ClaimLimitAmount { get; set; }
        public decimal? ThirdPartyLimitAmount { get; set; }
        public decimal? WageandHourLimitAmount { get; set; }
        public decimal? IRCALimitAmount { get; set; }
        public decimal? DeductibleAmount { get; set; }
        public decimal? WageandHourDeductibleAmount { get; set; }
        public decimal? IRCADeductibleAmount { get; set; }
        public decimal? ThirdPartyDeductibleAmount { get; set; }
        public decimal? DeductibleDiscountPercent { get; set; }
        public string IsMinimumPremiumPolicy { get; set; }
        public string EPLIState { get; set; }
        public EPLICoverageQuestions CoverageQuestions { get; set; }
    }

    public class EPLICoverageQuestions
    {
        public RatingInfo RatingInfo { get; set; }
        public string RatingState { get; set; }
        public int? NumberOfEmployees { get; set; }
        public string NumberOfEmployeesEffDte { get; set; }
        public decimal? Premium { get; set; }
    }

    public class RatingInfo
    {
        public string IsReferral { get; set; }
        public string RatingEffDate { get; set; }
        public decimal? RateFactor { get; set; }
        public string RatingGUID { get; set; }
        public string RatingGUIDEffDte { get; set; }
        public string SchemaVersion { get; set; }
    }

    public class NTL
    {
        public NTLCoverageQuestions CoverageQuestions { get; set; }
        public Exposures Exposures { get; set; }
    }

    public class NTLExcess
    {
        public NTLCoverageQuestions CoverageQuestions { get; set; }
        public Exposures Exposures { get; set; }
    }

    public class NTLCoverageQuestions
    {
        public decimal? Limits { get; set; }
        public decimal? PackagePolicyDiscountPercent { get; set; }
        public string PackagePolicyDiscountEffectiveDate { get; set; }
    }

    public class PhysicalDamage
    {
        public PhysicalDamageQuestions CoverageQuestions { get; set; }
        public Exposures Exposures { get; set; }
    }

    public class PhysicalDamageQuestions
    {
        public decimal? Deductible { get; set; }
        public string DeductibleChgEffDte { get; set; }
        public string PDAdjustmentFactorRatingGUID { get; set; }
    }

    public class Exposures
    {
        [XmlElement("Vehicle")]
        public List<ExposureVehicle> Vehicle { get; set; } = new();
    }

    public class ExposureVehicle
    {
        public string Vehicle_SequenceNo { get; set; }
        public string ExpEffectiveDate { get; set; } // some feeds use ExpEffectiveDate vs EffectiveDate
        public string EffectiveDate { get; set; }
        public decimal? Premium { get; set; }
        public string UMSelectionKey { get; set; }
        public string UMSelectionKeyChgEffDate { get; set; }
        public string IDCardNameSeqNumber { get; set; }

        public ExposureRegistrantInfo ExposureRegistrantInfo { get; set; }
        public ExposureAdditionalInterests ExposureAdditionalInterests { get; set; }

        public decimal? PackagePolicyDiscountAmount { get; set; }
        public decimal? PDAdjustmentDiscountAmount { get; set; }
    }

    public class ExposureAdditionalInterests
    {
        [XmlElement("ExposureAdditionalInterest")]
        public List<ExposureAdditionalInterest> ExposureAdditionalInterest { get; set; } = new();
    }

    public class ExposureAdditionalInterest
    {
        public string AdditionalInterest_SequenceNo { get; set; }
        public string EffectiveDate { get; set; }
    }

    public class ExposureRegistrantInfo
    {
        [XmlElement("ExposureRegistrant")]
        public List<ExposureRegistrant> ExposureRegistrant { get; set; } = new();
    }

    public class ExposureRegistrant
    {
        public string ExposureRegistrantSeqNumber { get; set; }
        public string RegistrantEffectiveDate { get; set; }
    }

    public class WorkComp
    {
        public RatingInfo RatingInfo { get; set; }
        public WorkCompCoverageQuestions CoverageQuestions { get; set; }
        public StateExposures StateExposures { get; set; }
        public Locations Locations { get; set; }
    }

    public class WorkCompCoverageQuestions
    {
        public ExperienceInformation ExperienceInformation { get; set; }
    }

    public class ExperienceInformation
    {
        public decimal? ExperienceModifier { get; set; }
        public string ExpModEffDte { get; set; }
    }

    public class StateExposures
    {
        [XmlElement("State")]
        public List<State> State { get; set; } = new();
    }

    public class State
    {
        public string StateName { get; set; }
        public string EffectiveDate { get; set; }
        public string Risk_BureauID { get; set; }
        public ExperienceInformation ExperienceInformation { get; set; }
        public NonOfficerOperations NonOfficerOperations { get; set; }
    }

    public class NonOfficerOperations
    {
        [XmlElement("NonOfficerOperation")]
        public List<NonOfficerOperation> NonOfficerOperation { get; set; } = new();
    }

    public class NonOfficerOperation
    {
        public string WorkerType { get; set; }
        public decimal? Payroll { get; set; }
        public int? NumberofEmployees { get; set; }
    }

    public class Locations
    {
        [XmlElement("Location")]
        public List<Location> Location { get; set; } = new();
    }

    public class Location
    {
        public string TerminalNumber { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpirationDate { get; set; }
    }

    public class InsurePay
    {
        public string BillMethod { get; set; }
        public string BillMethodEffectiveDate { get; set; }
        public string PayGoStatus { get; set; }
        public string PayGoRegisterByDate { get; set; }
    }
}