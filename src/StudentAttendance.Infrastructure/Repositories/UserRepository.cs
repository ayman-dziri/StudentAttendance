using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Collections;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;
using StudentAttendance.src.StudentAttendance.Infrastructure.Mappers;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly IMongoCollection<UserDocument> _collection;

        public UserRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<UserDocument>(CollectionNames.Users);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            var document = UserMapper.ToDocument(user);
            await _collection.InsertOneAsync(document, cancellationToken: ct);
        }

        public async Task<User?> GetUserByIdAsync(string id,  CancellationToken ct = default)
        {
            var document = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct); // on cherche l'objet si son id == l'id entré dans le parametre

            return UserMapper.ToDomain(document);
        }
        public async Task<List<User>> GetUsersAsync(CancellationToken ct = default)
        {
            var documents = await _collection.Find(_ => true).ToListAsync(ct); // avec "_ => true" on retourne true dans chaque objet de ce document (pas besoin de mettre une condition qu'elle doit etre verifiée alors on recupere toute la liste)
            return documents.Select(UserMapper.ToDomain).ToList(); // on map chaque objet de ce document vers l'entité User puis on renvoie ces objets sous forme d'une liste
        }

        //public async Task<bool> UpdateUserAsync(string id, User user, CancellationToken ct = default)
        //{
        //    var document = UserMapper.ToDocument(user); // on map l'entite User vers le document UserDocument
        //    var result = await _collection.ReplaceOneAsync(
        //        filter: x => x.Id == id, // condition pour trouver l'user qu'on souhaite le modifier
        //        replacement: document, // on le remplace par ce nouveau objet
        //        options: new ReplaceOptions { IsUpsert = false }, // on desactive cette option qui permet d'inserer cet objet au cas où il n'existe pas
        //        cancellationToken: ct
        //    );

        //    return result.MatchedCount > 0 && result.ModifiedCount > 0; // on retourne un boolean si [le nbr des documents de cette collection qui ont passer la condition en true ET le nbr des documents de cette collection qui ont été modifié > 0 ] (donc ce cas soit 1 soit 0)
        //}

        public async Task<bool> UpdateUserAsync(string id, User user, CancellationToken ct = default)
        {
            var document = UserMapper.ToDocument(user);

            var update = Builders<UserDocument>.Update
                .Set(x => x.FirstName, document.FirstName)
                .Set(x => x.LastName, document.LastName)
                .Set(x => x.BirthDate, document.BirthDate)
                .Set(x => x.GroupId, document.GroupId);

            var result = await _collection.UpdateOneAsync(
                x => x.Id == id,
                update,
                cancellationToken: ct
            );

            return result.MatchedCount > 0 && result.ModifiedCount > 0;
        }
        public async Task<bool> DeleteUserAsync(string id, CancellationToken ct = default)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id); // on supprime d'apres cette condition
            return result.DeletedCount > 0;
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _collection.Find(x => x.Email == email).FirstOrDefaultAsync(ct);

            return UserMapper.ToDomain(user);
        }

        public async Task<List<User>> GetStudentsByGroupIdAsync(string groupId, CancellationToken ct = default)
        {
            var filters = Builders<UserDocument>.Filter.And( // utilisant plusieurs filtres avec And
                    Builders<UserDocument>.Filter.Eq(x => x.Role, Role.STUDENT), // 1er filtre : juste qui ont le role STUDENT
                    Builders<UserDocument>.Filter.Eq(x => x.GroupId, groupId) // 2eme filtre : les users qui appartient à ce groupId
                );
            var users = await _collection.Find(filters).ToListAsync(ct); // on recuperant ces elements sous une liste

            var results =  users.Select(UserMapper.ToDomain).ToList(); // on retournant la liste des users en les mappant de document vers entités
            Console.WriteLine("Etudiants by Group : ", results);
            return results;
        }
    }
}
