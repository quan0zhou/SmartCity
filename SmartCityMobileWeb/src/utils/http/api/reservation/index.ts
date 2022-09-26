
import { get } from '@/utils/http/axios';
enum URL {
    tagList = '/reservation/tagList',
    tag='/reservation/tag',
    info='/reservation/info'
}

const getTagList = async () => get<any>({ url: URL.tagList });
const getTag = async (date:string) => get<any>({ url: URL.tag+'/'+date });
const getInfo = async (id:string) => get<any>({ url: URL.info+'/'+id });
export { getTagList,getTag,getInfo };
