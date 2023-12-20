
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using System;

    public class BasicUserValidationService:AspNetCore.Authentication.Basic.IBasicUserValidationService
    {
        private readonly ILogger<BasicUserValidationService> _logger;
        private readonly Aserto.TodoApp.IUserRepository _userRepository;

        public BasicUserValidationService(ILogger<BasicUserValidationService> logger, Aserto.TodoApp.IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<bool> IsValidAsync(string username, string password)
        {
            try
            {
                return await _userRepository.Authenticate(username, password);                
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
