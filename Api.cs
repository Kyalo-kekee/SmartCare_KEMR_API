using DataAccessLayer.Data;
using DataAccessLayer.MessageTypes.Common;
using DataAccessLayer.MessageTypes;
using Hangfire;
using Microsoft.Extensions.Options;

namespace SmartCare;

public static class Api
{
    
  
    public static void ConfigureApi(this WebApplication app )
    {
        app.MapPost("/Patient", ProcessRequest);

    }
    
    private static IResult ProcessRequest(
        AbstractPatientMessage patient,
        IProcessMessage repository,
        IOptions<AppSettingsMessageTypes> config)
    {
        MESSAGEHEADER? msgHeader = patient.MESSAGE_HEADER;
        msgHeader = msgHeader ?? throw new ArgumentNullException(nameof(msgHeader));

        
        if (msgHeader.MESSAGE_TYPE == config.Value.PatientRegistration)
        {
            var jobId = BackgroundJob.Schedule(() => repository.RegisterPatient(patient),
                TimeSpan.FromSeconds(3));
        }

        if(msgHeader.MESSAGE_TYPE == "RDE^001")
        {
           var job=  BackgroundJob.Schedule(() => repository.CreatePhamarcyOrder(patient),
               TimeSpan.FromSeconds(3));

            var job2 = BackgroundJob.Schedule(() => repository.PopulatePharmarcyOrder(patient),
                TimeSpan.FromSeconds(3));
        }
        return Results.Ok(msgHeader);

    }
}
