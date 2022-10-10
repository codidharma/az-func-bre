using AutoMapper;

namespace Az.Serverless.Bre.Func01.Mapper.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static IMapper Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(AutoMapperConfiguration).Assembly);
            });

            return config.CreateMapper();
        }
    }
}
