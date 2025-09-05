using Microsoft.Data.Sqlite;

public class Conta
{
    public string? NomeTitular;
    public double Saldo;

    public void CriarConta(string nomeTitular, SqliteConnection connection)
    {
        NomeTitular = nomeTitular;
        Saldo = 0;

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Conta (nomeTitular, saldo)
            VALUES ($nomeTitular, $saldo);
        ";
        cmd.Parameters.AddWithValue("$nomeTitular", NomeTitular);
        cmd.Parameters.AddWithValue("$saldo", Saldo);
        cmd.ExecuteNonQuery();

        Console.WriteLine("Conta criada com sucesso");
        Console.WriteLine("==============DADOS DA CONTA=============");
        Console.WriteLine($"Nome: {nomeTitular}\nSaldo: {Saldo}");
    }
    public void ConsultarSaldo(int conta, SqliteConnection connection)
    {
        var cmd = connection.CreateCommand();

        cmd.CommandText = @"
        SELECT nomeTitular, saldo
        FROM Conta
        WHERE numeroConta = $conta
        ";

        cmd.Parameters.AddWithValue("$conta", conta);

        var retorno = cmd.ExecuteReader();

        try
        {
            while (retorno.Read())
            {
                var nome = retorno.GetString(0);
                var saldo = retorno.GetDouble(1);
                Console.WriteLine($"Olá, {nome}. Seu saldo é de {saldo}");
            }

        }
        finally
        {
            retorno.Close();
            retorno.DisposeAsync();
        }
    }
}
