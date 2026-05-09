using FluentAssertions;

using Domain.Common;

namespace UnitTests.Domain.Common;

public class EntityTests
{
    private sealed class FakeEntity : Entity
    {
        public FakeEntity()
        { }

        public FakeEntity(Guid id)
        {
            Id = id;
        }
    }

    private sealed class OtherFakeEntity : Entity
    {
        public OtherFakeEntity()
        { }

        public OtherFakeEntity(Guid id)
        {
            Id = id;
        }
    }

    [Fact]
    public void Deve_Gerar_Id_Automaticamente()
    {
        var entity = new FakeEntity();

        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Deve_Considerar_Entidades_Iguais_Quando_Ids_Forem_Iguais()
    {
        var id = Guid.NewGuid();

        var entity1 = new FakeEntity(id);
        var entity2 = new FakeEntity(id);

        entity1.Should().Be(entity2);
    }

    [Fact]
    public void Deve_Considerar_Entidades_Diferentes_Quando_Ids_Forem_Diferentes()
    {
        var entity1 = new FakeEntity(Guid.NewGuid());
        var entity2 = new FakeEntity(Guid.NewGuid());

        entity1.Should().NotBe(entity2);
    }

    [Fact]
    public void Operador_Igual_Deve_Retornar_True_Quando_Ids_Forem_Iguais()
    {
        var id = Guid.NewGuid();

        var entity1 = new FakeEntity(id);
        var entity2 = new FakeEntity(id);

        var result = entity1 == entity2;

        result.Should().BeTrue();
    }

    [Fact]
    public void Operador_Diferente_Deve_Retornar_True_Quando_Ids_Forem_Diferentes()
    {
        var entity1 = new FakeEntity(Guid.NewGuid());
        var entity2 = new FakeEntity(Guid.NewGuid());

        var result = entity1 != entity2;

        result.Should().BeTrue();
    }

    [Fact]
    public void Deve_Retornar_Mesmo_HashCode_Quando_Ids_Forem_Iguais()
    {
        var id = Guid.NewGuid();

        var entity1 = new FakeEntity(id);
        var entity2 = new FakeEntity(id);

        var hash1 = entity1.GetHashCode();
        var hash2 = entity2.GetHashCode();

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Deve_Considerar_Referencia_Igual()
    {
        var entity = new FakeEntity();

        entity.Equals(entity).Should().BeTrue();
    }

    [Fact]
    public void Deve_Retornar_False_Quando_Objeto_For_Null()
    {
        var entity = new FakeEntity();

        var result = entity.Equals(null);

        result.Should().BeFalse();
    }

    [Fact]
    public void Deve_Retornar_False_Quando_Tipos_Forem_Diferentes()
    {
        var entity = new FakeEntity();

        var result = entity.Equals("teste");

        result.Should().BeFalse();
    }

    [Fact]
    public void Deve_Retornar_False_Quando_Ids_Iaguais_Mas_Tipos_Forem_Diferentes()
    {
        var entity1 = new FakeEntity();
        var entity2 = new OtherFakeEntity(entity1.Id);

        var result = entity1.Equals(entity2);

        result.Should().BeFalse();
    }
}