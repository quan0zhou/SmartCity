
import { get } from '@/utils/http/axios';
enum URL {
    tagList = '/reservation/tagList',
    tag='/reservation/tag'
}

const getTagList = async () => get<any>({ url: URL.tagList });
const getTag = async (date:string) => get<any>({ url: URL.tag+'/'+date });
export { getTagList,getTag };
