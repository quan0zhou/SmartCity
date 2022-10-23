
import { get } from '@/utils/http/axios';
enum URL {
  info = '/custSpace/info',
  openId = '/custSpace/openId'
}

export interface LoginData {
  username: string;
  password: string;
}

const getSpaceInfo = async () => get<any>({ url: URL.info });
const getOpenId = async (code:string) => get<any>({ url: URL.openId+'/'+code });
export { getSpaceInfo,getOpenId };
