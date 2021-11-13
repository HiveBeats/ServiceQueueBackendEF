using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Features.Services.Services;
using WebApi.Features.Services.Responses;
using WebApi.Features.Services.Requests;
using System;
using System.Linq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WebApi.Tests.Features.Services
{
    public class ServicesServiceTests
    {
        private readonly ServiceCollection _services;
        private readonly ServiceProvider _serviceProvider;

        public ServicesServiceTests()
        {
            _services = new ServiceCollection();

            _services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "ServicesInMemoryDb"), 
               ServiceLifetime.Scoped, 
               ServiceLifetime.Scoped);

            _services.AddScoped<IServicesService, ServicesService>();

            _serviceProvider = _services.BuildServiceProvider();
        }

        [Fact]
        public async Task CreateServiceParentNotExistsThrow()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                Random rnd = new Random();

                var request = new CreateServiceRequest()
                {
                    ParentId = rnd.Next(100, 10000),
                    Name = Guid.NewGuid().ToString()
                };

                //act && assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateService(request));
            }
        }

        [Fact]
        public async Task CreateServiceWithoutParentSuccessfull()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                
                var request = new CreateServiceRequest()
                {
                    Name = Guid.NewGuid().ToString()
                };
                
                //act
                var result = await service.CreateService(request);

                //assert
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var foundedItem = await db.Services.FirstOrDefaultAsync(x => x.Name == request.Name);
                
                Assert.NotNull(foundedItem);
            }
        }

        [Fact]
        public async Task CreateServiceSuccessfullResponse()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                
                var request = new CreateServiceRequest()
                {
                    Name = Guid.NewGuid().ToString()
                };
                
                //act
                var result = await service.CreateService(request);

                //assert
                Assert.NotNull(result?.Id);
            }
        }

        [Fact]
        public async Task CreateServiceResponseCorrectId()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                
                var request = new CreateServiceRequest()
                {
                    Name = Guid.NewGuid().ToString()
                };
                
                //act
                var result = await service.CreateService(request);

                //assert
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var foundedItem = await db.Services.FirstOrDefaultAsync(x => x.Name == request.Name);
                
                Assert.Equal(foundedItem.Id, result.Id);
            }
        }

        [Fact]
        public async Task CreateServiceWithParentBeChild()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var parentRequest = new CreateServiceRequest()
                {
                    Name = Guid.NewGuid().ToString()
                };
                
                var parentResult = await service.CreateService(parentRequest);

                var childRequest = new CreateServiceRequest()
                {
                    Name = Guid.NewGuid().ToString(),
                    ParentId = parentResult.Id
                };

                //act
                var childResult = await service.CreateService(childRequest);

                //assert
                var foundedItem = await db.Services.FindAsync(childResult.Id);
                Assert.Equal(parentResult.Id, foundedItem.ParentId);
            }
        }

        [Fact]
        public async Task DeleteServiceNotExistsThrows()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                Random rnd = new Random();

                //act && assert
                await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteService(rnd.Next(100, 10000)));
            }
        }

        [Fact]
        public async Task DeleteServiceSuccessfully()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var request = new CreateServiceRequest()
                {
                    Name = Guid.NewGuid().ToString()
                };
                
                var item = await service.CreateService(request);
                
                //act
                var response = await service.DeleteService(item.Id);
                
                //assert
                var deletedItem = await db.Services.FindAsync(item.Id);
                Assert.Null(deletedItem);
            }
        }

        [Fact]
        public async Task DeleteServiceCorrectResponse()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var request = new CreateServiceRequest()
                {
                    Name = Guid.NewGuid().ToString()
                };
                
                var item = await service.CreateService(request);
                
                //act
                var response = await service.DeleteService(item.Id);
                
                //assert
                Assert.Equal(item.Id, response.Id);
            }
        }

        private async Task ClearServiceList(AppDbContext db)
        {
            var items = await db.Services.ToListAsync();
            db.Services.RemoveRange(items);
            await db.SaveChangesAsync();
        }

        private async Task CreateTree(IServicesService service)
        {
            var rootRequest = new CreateServiceRequest() { Name = "Root" };
            var rootItem = await service.CreateService(rootRequest);

            var childItemRequest = new CreateServiceRequest() { Name = "ChildOfRoot", ParentId = rootItem.Id };
            var childItem = await service.CreateService(childItemRequest);

            var childItemTwoRequest = new CreateServiceRequest() { Name = "ChildOfRootTwo", ParentId = rootItem.Id };
            var childItemTwo = await service.CreateService(childItemTwoRequest);

            var childItemChildRequest = new CreateServiceRequest() { Name = "ChildItemChild", ParentId = childItem.Id };
            var childItemChild = await service.CreateService(childItemChildRequest);
        }

        [Fact]
        public async Task GetTreeRootCorrect()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                await ClearServiceList(db);
                await CreateTree(service);

                //act
                var tree = await service.GetServiceTree();
                
                //assert
                var root = await db.Services.FirstOrDefaultAsync(x => x.Name == "Root");
                Assert.Equal(root.Id, tree.root.FirstOrDefault().key);
            }
        }

        [Fact]
        public async Task GetTreeChildCorrect()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                await ClearServiceList(db);
                await CreateTree(service);

                //act
                var tree = await service.GetServiceTree();
                
                //assert
                var child = await db.Services.FirstOrDefaultAsync(x => x.Name == "ChildOfRoot");
                Assert.Equal(child.Id, tree.root.FirstOrDefault().children.FirstOrDefault(x => x.label == "ChildOfRoot").key);
            }
        }

        [Fact]
        public async Task GetTreeRootHasAllChilds()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //arrange
                var service = scope.ServiceProvider.GetRequiredService<IServicesService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                await ClearServiceList(db);
                await CreateTree(service);

                //act
                var tree = await service.GetServiceTree();
                
                //assert
                var root = await db.Services.FirstOrDefaultAsync(x => x.Name == "Root");
                var rootChildsCount = await db.Services.Where(x => x.ParentId == root.Id).CountAsync();
                Assert.Equal(rootChildsCount, tree.root.FirstOrDefault().children.Count());
            }
        }
    }
}