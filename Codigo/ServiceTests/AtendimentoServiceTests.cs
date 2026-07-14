using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace ServiceTests
{
    [TestClass]
    public class AtendimentoServiceTests
    {
        private DeliFitContext context;
        private IAtendimentoService atendimentoService;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<DeliFitContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new DeliFitContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Atendimentos.AddRange(
                new Atendimento
                {
                    Id = 1,
                    DiaSemana = "1",
                    HorarioInicio = new DateTime(2024, 6, 10, 8, 0, 0),
                    HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0),
                    IdRestaurante = 1
                },
                new Atendimento
                {
                    Id = 2,
                    DiaSemana = "2",
                    HorarioInicio = new DateTime(2024, 6, 11, 8, 0, 0),
                    HorarioFim = new DateTime(2024, 6, 11, 18, 0, 0),
                    IdRestaurante = 1
                },
                new Atendimento
                {
                    Id = 3,
                    DiaSemana = "3",
                    HorarioInicio = new DateTime(2024, 6, 12, 8, 0, 0),
                    HorarioFim = new DateTime(2024, 6, 12, 18, 0, 0),
                    IdRestaurante = 1
                }
            );

            context.SaveChanges();

            atendimentoService = new AtendimentoService(context);
        }

        [TestMethod]
        public void CreateTest()
        {
            atendimentoService.Create(new Atendimento
            {
                Id = 4,
                DiaSemana = "4",
                HorarioInicio = new DateTime(2024, 6, 13, 8, 0, 0),
                HorarioFim = new DateTime(2024, 6, 13, 18, 0, 0),
                IdRestaurante = 1
            });

            Assert.AreEqual(4, atendimentoService.GetAll(1).Count());

            var atendimento = atendimentoService.Get(4);
            Assert.IsNotNull(atendimento);
            Assert.AreEqual("4", atendimento.DiaSemana);
        }

        [TestMethod]
        public void CreateTest_DiaDuplicado_LancaServiceException()
        {
            var duplicado = new Atendimento
            {
                DiaSemana = "1",
                HorarioInicio = new DateTime(2024, 6, 10, 9, 0, 0),
                HorarioFim = new DateTime(2024, 6, 10, 17, 0, 0),
                IdRestaurante = 1
            };

            Assert.ThrowsException<ServiceException>(() => atendimentoService.Create(duplicado));
        }

        [TestMethod]
        public void CreateTest_HorarioInicioMaiorQueFim_LancaServiceException()
        {
            var invalido = new Atendimento
            {
                DiaSemana = "5",
                HorarioInicio = new DateTime(2024, 6, 14, 18, 0, 0),
                HorarioFim = new DateTime(2024, 6, 14, 8, 0, 0),
                IdRestaurante = 1
            };

            Assert.ThrowsException<ServiceException>(() => atendimentoService.Create(invalido));
        }

        [TestMethod]
        public void CreateTest_HorarioInicioIgualFim_LancaServiceException()
        {
            var invalido = new Atendimento
            {
                DiaSemana = "5",
                HorarioInicio = new DateTime(2024, 6, 14, 12, 0, 0),
                HorarioFim = new DateTime(2024, 6, 14, 12, 0, 0),
                IdRestaurante = 1
            };

            Assert.ThrowsException<ServiceException>(() => atendimentoService.Create(invalido));
        }

        [TestMethod]
        public void DeleteTest()
        {
            atendimentoService.Delete(2);

            Assert.AreEqual(2, atendimentoService.GetAll(1).Count());
            Assert.IsNull(atendimentoService.Get(2));
        }

        [TestMethod]
        public void EditTest()
        {
            var atendimento = atendimentoService.Get(3);

            atendimento.DiaSemana = "4";

            atendimentoService.Edit(atendimento);

            atendimento = atendimentoService.Get(3);

            Assert.IsNotNull(atendimento);
            Assert.AreEqual("4", atendimento.DiaSemana);
        }

        [TestMethod]
        public void EditTest_DiaDuplicado_LancaServiceException()
        {
            var atendimento = atendimentoService.Get(3);
            atendimento.DiaSemana = "1";

            Assert.ThrowsException<ServiceException>(() => atendimentoService.Edit(atendimento));
        }

        [TestMethod]
        public void EditTest_HorarioInicioMaiorQueFim_LancaServiceException()
        {
            var atendimento = atendimentoService.Get(1);
            atendimento.HorarioInicio = new DateTime(2024, 6, 10, 20, 0, 0);
            atendimento.HorarioFim = new DateTime(2024, 6, 10, 8, 0, 0);

            Assert.ThrowsException<ServiceException>(() => atendimentoService.Edit(atendimento));
        }

        [TestMethod]
        public void GetTest()
        {
            var atendimento = atendimentoService.Get(1);

            Assert.IsNotNull(atendimento);
            Assert.AreEqual("1", atendimento.DiaSemana);
            Assert.AreEqual((uint)1, atendimento.IdRestaurante);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var lista = atendimentoService.GetAll(1);

            Assert.IsNotNull(lista);
            Assert.AreEqual(3, lista.Count());

            var primeiro = lista.First();
            Assert.AreEqual((uint)1, primeiro.Id);
            Assert.AreEqual("1", primeiro.DiaSemana);
        }
    }
}
