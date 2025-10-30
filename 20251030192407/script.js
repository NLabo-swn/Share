window.onload = async () => {
    const response = await fetch('./character.json');
    const data = await response.json();
    const character = document.getElementsByClassName("character")[0];

    for (let i = 0; i < data.length; i++) {
        const characterElement = await createCharacterElement(data[i]);
        character.appendChild(characterElement);
    }
};

/**
 * キャラクターの要素を作成する
 * @param {*} jsonData 読み込んだJSONデータ
 */
async function createCharacterElement(jsonData) {
    // アニメーションの設定を作成
    const animationName = createKeyFrameSettings(jsonData);
    const animationDuration = 9;

    // キャラクター要素を作成
    const characterElement = document.createElement('div');
    characterElement.classList.add("tmp");

    // キャラクターの画像要素を作成
    const img = document.createElement("div");
    img.classList.add("img");
    img.style.backgroundImage = `url(${jsonData.image[0]})`;
    img.style.animation = `${animationName} ${animationDuration}s infinite`;

    // キャラクターの名前要素を作成
    const name = document.createElement("div");
    name.classList.add("name");

    // 名前にリンクを設定
    const url = document.createElement("a");
    url.href = jsonData.url;
    url.textContent = jsonData.name;
    name.appendChild(url);

    // キャラクター要素に画像と名前を追加
    characterElement.appendChild(img);
    characterElement.appendChild(name);

    return characterElement;
}

/**
 * キーフレームの設定を作成する
 * @param {*} jsonData 読み込んだJSONデータ
 * @returns {string} アニメーション名
 */
function createKeyFrameSettings(jsonData) {
    // スタイル要素を作成してキーフレームを追加
    const style = document.createElement('style');
    style.type = 'text/css';

    // 画像の枚数に応じてキーフレームを生成
    const rate = 100 / jsonData.image.length;
    const animationName = `${jsonData.name}_Loop`;
    const keyFrames = `
    @keyframes ${animationName} {
        ${jsonData.image.map((img, index) => {
        return `${Math.min(index * rate, 100)}% { background-image: url(${img})(); }`;
    }).join('\n')}
        100% { background-image: url(${jsonData.image[0]}); }
    }`;

    // スタイル要素にキーフレームを追加
    style.innerHTML = keyFrames;
    document.head.appendChild(style);

    return animationName;
}