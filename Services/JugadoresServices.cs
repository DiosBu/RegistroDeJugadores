using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RegistroDeJugadores.DAL;
using RegistroDeJugadores.Models;


namespace RegistroDeJugadores.Services
{
    public class JugadoresServices(IDbContextFactory<Contexto> dbContextFactory)
    {
        private readonly IDbContextFactory<Contexto> ContextFactory;

        // MÉTODO GUARDAR
        public async Task<bool> Guardar(Jugadores jugador)
            => await Existe(jugador.JugadorId)
               ? await Modificar(jugador)
               : await Insertar(jugador);

        // MÉTODO EXISTE
        public async Task<bool> Existe(int id)
        {
            using var ctx = await ContextFactory.CreateDbContextAsync();
            return await ctx.Jugadores.AnyAsync(e => e.JugadorId == id);
        }

        // MÉTODO INSERTAR  (valida duplicado por Nombres)
        public async Task<bool> Insertar(Jugadores jugador)
        {
            using var ctx = await ContextFactory.CreateDbContextAsync();

            // Validación de negocio: no permitir dos jugadores con el mismo nombre
            if (await ctx.Jugadores.AnyAsync(j => j.Nombres == jugador.Nombres))
                throw new InvalidOperationException("Ya existe un jugador con ese nombre.");

            ctx.Jugadores.Add(jugador);
            return await ctx.SaveChangesAsync() > 0;
        }

        // MÉTODO MODIFICAR  (valida duplicado por Nombres excluyendo el mismo Id)
        public async Task<bool> Modificar(Jugadores jugador)
        {
            using var ctx = await ContextFactory.CreateDbContextAsync();

            if (await ctx.Jugadores.AnyAsync(j =>
                j.Nombres == jugador.Nombres && j.JugadorId != jugador.JugadorId))
                throw new InvalidOperationException("Ya existe un jugador con ese nombre.");

            ctx.Entry(jugador).State = EntityState.Modified;
            return await ctx.SaveChangesAsync() > 0;
        }

        // MÉTODO BUSCAR
        public async Task<Jugadores?> Buscar(int id)
        {
            using var ctx = await ContextFactory.CreateDbContextAsync();
            return await ctx.Jugadores.FindAsync(id);
        }

        // MÉTODO ELIMINAR
        public async Task<bool> Eliminar(int id)
        {
            using var ctx = await ContextFactory.CreateDbContextAsync();
            var entidad = await ctx.Jugadores.FindAsync(id);
            if (entidad is null) return false;

            ctx.Jugadores.Remove(entidad);
            return await ctx.SaveChangesAsync() > 0;
        }

        // MÉTODO LISTAR
        public async Task<List<Jugadores>> Listar(Expression<Func<Jugadores, bool>> criterio)
        {
            using var ctx = await ContextFactory.CreateDbContextAsync();
            return await ctx.Jugadores
                            .Where(criterio)
                            .OrderBy(e => e.Nombres)
                            .ToListAsync();
        }
    }
}
