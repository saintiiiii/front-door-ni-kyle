let csrf='';
export async function api<T=any>(path:string,opts:RequestInit={}):Promise<T>{
 const method=(opts.method||'GET').toUpperCase(),headers=new Headers(opts.headers);
 if(!(opts.body instanceof FormData)&&opts.body)headers.set('Content-Type','application/json');
 if(!['GET','HEAD','OPTIONS'].includes(method)){if(!csrf){const t=await fetch('/api/auth/csrf',{credentials:'same-origin'});csrf=(await t.json()).token;}headers.set('X-XSRF-TOKEN',csrf);}
 const response=await fetch(path,{...opts,method,headers,credentials:'same-origin'});
 if(response.status===204)return undefined as T;
 const body=await response.json().catch(()=>({message:'Something went wrong. Please try again.'}));
 if(!response.ok)throw new Error(body.message||body.title||body.errors?.join?.(' ')||'Request failed. Please try again.');
 return body as T;
}
export const money=(n:number)=>new Intl.NumberFormat('en-PH',{style:'currency',currency:'PHP',maximumFractionDigits:0}).format(n);
