using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
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
                    DiaSemana = "Domingo",
                    HorarioInicio = new DateTime(2024, 6, 10, 8, 0, 0),
                    HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0),
                    IdRestaurante = 1
                },
                new Atendimento
                {
                    Id = 2,
                    DiaSemana = "Segunda-feira",
                    HorarioInicio = new DateTime(2024, 6, 11, 8, 0, 0),
                    HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0),
                    IdRestaurante = 1
                },
                new Atendimento
                {
                    Id = 3,
                    DiaSemana = "Terça-feira",
                    HorarioInicio = new DateTime(2024, 6, 12, 8, 0, 0),
                    HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0),
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
                DiaSemana = "Terça-feira",
                HorarioInicio = new DateTime(2024, 6, 12, 8, 0, 0),
                HorarioFim = new DateTime(2024, 6, 12, 18, 0, 0),
                IdRestaurante = 1
            });

            Assert.AreEqual(4, atendimentoService.GetAll(1).Count());

            var atendimento = atendimentoService.Get(4);
            Assert.IsNotNull(atendimento);
            Assert.AreEqual("Terça-feira", atendimento.DiaSemana);
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

            atendimento.DiaSemana = "Quarta-feira";

            atendimentoService.Edit(atendimento);

            atendimento = atendimentoService.Get(3);

            Assert.IsNotNull(atendimento);
            Assert.AreEqual("Quarta-feira", atendimento.DiaSemana);
        }

        [TestMethod]
        public void GetTest()
        {
            var atendimento = atendimentoService.Get(1);

            Assert.IsNotNull(atendimento);
            Assert.AreEqual("Domingo", atendimento.DiaSemana);
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
            Assert.AreEqual("Domingo", primeiro.DiaSemana);
        }
    }
}