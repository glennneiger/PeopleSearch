import { HttpServiceBase } from './http-service-base';
import { Stats } from 'models/stats';
import { Person } from 'models/person';

export class PeopleService extends HttpServiceBase {
    async getStats(): Promise<Stats> {
        let requestUrl = `${this.rootUrl}/people/data/stats`;
        const { content } = await this.httpClient.get(requestUrl);
        return content;
    }

    async find(search:string, skip:number, take:number, goSlow: boolean): Promise<Person[]> {
        let requestUrl = `${this.rootUrl}/people/data/find?search=${search}&skip=${skip}&take=${take}&goSlow=${goSlow}`;
        const { content } = await this.httpClient.get(requestUrl);
        return content;
    }

    async getById(id): Promise<Person> {
        let requestUrl = `${this.rootUrl}/people/data/${id}`;
        const { content } = await this.httpClient.get(requestUrl);
        return content;
    }

    async create(person: Person): Promise<Person> {
        let requestUrl = `${this.rootUrl}/people/data`;
        const { content } = await this.httpClient.post(requestUrl, person);
        return content;
    }

    async save(person: Person): Promise<void> {
        let requestUrl = `${this.rootUrl}/people/data`;
        await this.httpClient.put(requestUrl, person);
    }
    
    async delete(id): Promise<void> {
        let requestUrl = `${this.rootUrl}/people/data/${id}`;
        await this.httpClient.delete(requestUrl);
    }


    async count(search: string = null): Promise<number> {
        let requestUrl = `${this.rootUrl}/people/data/count?search=${search}`;
        const { content } = await this.httpClient.get(requestUrl);
        return content;
    }
}