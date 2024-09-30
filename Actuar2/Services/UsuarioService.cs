using Actuar2.Dto;
using Actuar2.Models;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.ResponseCompression;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Actuar2.Services
{
    public class UsuarioService : IUsuarioInterface
    {

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UsuarioService(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<ResponseModel<UsuarioListarDto>> BuscarUsuarioPorId(int usuarioId)
        {
            ResponseModel<UsuarioListarDto> response = new ResponseModel<UsuarioListarDto>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuarioBanco = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "SELECT * FROM Usuarios WHERE id = @Id", new { Id = usuarioId });

                if (usuarioBanco == null)
                {
                    response.Mensagem = "Nenhum usuário localizado!";
                    response.Status = false;
                    return response;
                }

                // Mapear diretamente o único objeto
                response.Dados = _mapper.Map<UsuarioListarDto>(usuarioBanco);
                response.Mensagem = "Usuário localizado com sucesso!";  
            }

            return response;
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> BuscarUsuarios()
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {

                var usuariosBanco = await connection.QueryAsync<Usuario>("select * from Usuarios");

                if (usuariosBanco.Count() == 0)
                {
                    response.Mensagem = "Nenhum usuário localizado!";
                    response.Status = false;
                    return response;
                }

                //Transformar Mapper
                var usuarioMapeado = _mapper.Map<List<UsuarioListarDto>>(usuariosBanco);

                response.Dados = usuarioMapeado;
                response.Mensagem = "Usuários localizados com sucesso!"; 

            }

            return response;
           
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> CriarUsuario(UsuarioCriarDto usuarioCriarDto)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuariosBanco = await connection.ExecuteAsync("insert into Usuarios (NomeCompleto, Email, Cargo, Salario, CPF, Senha, Situacao) " +
                    "values (@NomeCompleto, @Email, @Cargo, @Salario, @CPF, @Senha, @Situacao)", usuarioCriarDto);

                if(usuariosBanco == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao realizar o registro!";
                    response.Status = false;
                    return response;
                }

                var usuarios = await ListarUsuarios(connection);

                var usuariosMapeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);

                response.Dados = usuariosMapeados;
                response.Mensagem = "Usuários listados com sucesso!";

            }

            return response;
        }

        private static async Task<IEnumerable<Usuario>> ListarUsuarios(SqlConnection connection)
        {

            return await connection.QueryAsync<Usuario>("select * from Usuarios");

        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> EditarUsuario(UsuarioEditarDto usuarioEditarDto)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {

                var usuariosBanco = await connection.ExecuteAsync("update Usuarios set NomeCompleto = @NomeCompleto, Email = @Email, Cargo = @Cargo, Salario = @Salario, Situacao = @Situacao, CPF = @CPF where Id = @Id ", usuarioEditarDto);


                if(usuariosBanco == 0)
                {
                    response.Mensagem = "Ocorreu um erro a realizar a edição!";
                    response.Status = false;
                    return response;
                }


                var usuarios = await ListarUsuarios(connection);
                var usuariosMapeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);

                response.Dados = usuariosMapeados;
                response.Mensagem = "Usuários listados com sucesso!";


            }

            return response;
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> RemoverUsuario(int usuarioId)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using(var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuariosBanco = await connection.ExecuteAsync("delete from Usuarios where id =@Id", new {Id = usuarioId });
            
                if (usuariosBanco == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao realizar a edição!";
                    response.Status = false;
                    return response;
                }


                var usuarios = await ListarUsuarios(connection);

                var usuariosMapeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);

                response.Dados = usuariosMapeados;
                response.Mensagem = "Usuários listados com sucesso!";
            
            
            }

            return response;
        }
    }
}